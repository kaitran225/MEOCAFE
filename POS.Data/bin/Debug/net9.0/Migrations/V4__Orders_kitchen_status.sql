-- Kitchen display: order status (pending, sent, done)
ALTER TABLE orders ADD COLUMN IF NOT EXISTS kitchen_status TEXT NOT NULL DEFAULT 'pending';

CREATE INDEX IF NOT EXISTS ix_orders_kitchen_status ON orders(kitchen_status);
