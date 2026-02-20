-- Customer email/address; link order to customer
ALTER TABLE customer ADD COLUMN IF NOT EXISTS email TEXT;
ALTER TABLE customer ADD COLUMN IF NOT EXISTS address TEXT;

ALTER TABLE orders ADD COLUMN IF NOT EXISTS customer_id INTEGER REFERENCES customer(id);
