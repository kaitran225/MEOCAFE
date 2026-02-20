-- Seed initial admin account for first-time login.
-- Only runs once; safe if employee table already has rows (insert ignored when username exists).

INSERT INTO employee (fullname, username, password, phone_number, gender, address, dob, role)
SELECT 'Administrator', 'admin', '123456', NULL, NULL, NULL, NULL, 'Manager'
WHERE NOT EXISTS (SELECT 1 FROM employee WHERE username = 'admin');
