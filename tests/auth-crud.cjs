const https = require('node:https');
const assert = require('node:assert/strict');
const { spawn, execFileSync } = require('node:child_process');
const { randomUUID } = require('node:crypto');
const path = require('node:path');
const project = path.resolve(__dirname, '../ASPDOTNET-MVC-CRUD');
const app = path.join(project, 'bin/Debug/net8.0/ASPDOTNET-MVC-CRUD.exe');
const suffix = randomUUID();
const password = `Test!Aa9-${randomUUID()}`;
const adminEmail = `admin-${suffix}@example.test`;
const viewerEmail = `viewer-${suffix}@example.test`;
const env = { ...process.env, ASPNETCORE_ENVIRONMENT: 'Development',
  ConnectionStrings__DemoConnectionString: 'Server=.\\SQLEXPRESS;Database=MvcCrudAuthTest;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True' };
execFileSync(app, ['--create-admin'], { cwd: project, env: { ...env, Admin__Email: adminEmail, Admin__Password: password }, stdio: 'pipe' });
const server = spawn(app, ['--urls', 'https://localhost:7255'], { cwd: project, env, windowsHide: true, stdio: 'pipe' });
let logs = '';
server.stdout.on('data', d => logs += d);
server.stderr.on('data', d => logs += d);
function client() {
  const cookies = new Map();
  return async function request(route, data) {
    return new Promise((resolve, reject) => {
      const body = data === undefined ? null : new URLSearchParams(data).toString();
      const req = https.request(`https://localhost:7255${route}`, {
        rejectUnauthorized: false, method: body === null ? 'GET' : 'POST',
        headers: { Cookie: [...cookies].map(([k,v]) => `${k}=${v}`).join('; '),
          ...(body === null ? {} : { 'Content-Type': 'application/x-www-form-urlencoded', 'Content-Length': Buffer.byteLength(body) }) }
      }, res => {
        for (const value of res.headers['set-cookie'] || []) {
          const [key, ...parts] = value.split(';')[0].split('='); cookies.set(key, parts.join('='));
        }
        let html = ''; res.on('data', d => html += d);
        res.on('end', () => resolve({ status: res.statusCode, location: res.headers.location, html, headers: res.headers }));
      });
      req.on('error', reject); req.end(body);
    });
  };
}
function token(response) {
  const value = response.html.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/);
  assert.ok(value, 'antiforgery token rendered'); return value[1];
}
async function form(c, route, data) {
  return c(route, { ...data, __RequestVerificationToken: token(await c(route)) });
}
(async () => {
  try {
    const guest = client();
    for (let i = 0; i < 50; i++) {
      try { if ((await guest('/Account/Login')).status === 200) break; } catch {}
      if (i === 49) throw new Error('Test server did not start: ' + logs);
      await new Promise(r => setTimeout(r, 200));
    }
    for (const route of ['/', '/Employees', '/Employees/Add']) {
      const r = await guest(route); assert.equal(r.status, 302); assert.match(r.location, /Account\/Login/);
    }
    assert.equal((await guest('/Account/Register', { Email: viewerEmail, Password: password })).status, 400);
    const viewer = client();
    const registered = await form(viewer, '/Account/Register', { Email: viewerEmail, Password: password, ConfirmPassword: password, Role: 'Admin' });
    assert.equal(registered.status, 302);
    assert.equal((await viewer('/Employees')).status, 200);
    assert.ok(!(await viewer('/Employees')).html.includes('href="/Employees/Add"'));
    for (const route of ['/Employees/Add', '/Employees/Edit/00000000-0000-0000-0000-000000000001']) {
      assert.match((await viewer(route)).location, /AccessDenied/);
    }
    const viewerToken = token(await viewer('/'));
    for (const route of ['/Employees/Add', '/Employees/Edit', '/Employees/Delete']) {
      assert.match((await viewer(route, { __RequestVerificationToken: viewerToken })).location, /AccessDenied/);
    }
    assert.equal((await viewer('/Account/AccessDenied')).status, 403);
    const admin = client();
    assert.equal((await form(admin, '/Account/Login', { Email: adminEmail, Password: password, ReturnUrl: 'https://example.com' })).location, '/');
    const employee = { Name: `CRUD ${suffix}`, Email: 'employee@example.test', Salary: '50000', Department: 'QA', DateOfBirth: '1995-06-15' };
    assert.equal((await admin('/Employees/Add', employee)).status, 400);
    const invalid = await form(admin, '/Employees/Add', { ...employee, DateOfBirth: '2999-01-01' });
    assert.equal(invalid.status, 200); assert.match(invalid.html, /cannot be in the future/);
    assert.equal((await form(admin, '/Employees/Add', employee)).status, 302);
    const listing = await admin('/Employees');
    const row = listing.html.split('<tr>').find(s => s.includes(employee.Name));
    assert.ok(row);
    const id = row.match(/\/Employees\/Details\/([a-f0-9-]+)/)[1];
    assert.match((await viewer(`/Employees/Details/${id}`)).html, /employee@example.test/);
    const edited = await form(admin, `/Employees/Edit/${id}`, { ...employee, Id: id, Name: 'Updated employee', Salary: '60000' });
    assert.equal(edited.status, 302);
    assert.match((await admin(`/Employees/Details/${id}`)).html, /Updated employee/);
    const deleted = await admin('/Employees/Delete', { Id: id, __RequestVerificationToken: token(await admin(`/Employees/Edit/${id}`)) });
    assert.equal(deleted.status, 302);
    assert.equal((await admin(`/Employees/Details/${id}`)).status, 404);
    assert.equal((await admin('/Employees/Delete', { Id: id, __RequestVerificationToken: token(await admin('/')) })).status, 404);
    assert.equal((await admin('/Account/Logout', { __RequestVerificationToken: token(await admin('/')) })).status, 302);
    assert.match((await admin('/Employees')).location, /Account\/Login/);
    assert.equal((await viewer('/Account/Logout', { __RequestVerificationToken: token(await viewer('/')) })).status, 302);
    const duplicate = await form(guest, '/Account/Register', { Email: viewerEmail, Password: password, ConfirmPassword: password });
    assert.equal(duplicate.status, 200); assert.match(duplicate.html, /Could not create/);
    for (let i = 0; i < 5; i++) await form(guest, '/Account/Login', { Email: viewerEmail, Password: 'Wrong!Password1' });
    const locked = await form(guest, '/Account/Login', { Email: viewerEmail, Password: password });
    assert.equal(locked.status, 200); assert.match(locked.html, /Unable to sign in/);
    console.log('PASS: registration, login, logout, roles, privilege escalation rejection, CSRF, return URLs, lockout, validation, and full employee CRUD.');
  } finally { server.kill(); }
})().catch(error => { console.error(error); process.exitCode = 1; });
