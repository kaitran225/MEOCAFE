-- Seed demo categories and 30 drinks for MEOCAFE POS (run once per DB).

-- Categories (insert only if missing)
INSERT INTO categories (name) SELECT 'Coffee' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Coffee');
INSERT INTO categories (name) SELECT 'Tea' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Tea');
INSERT INTO categories (name) SELECT 'Cold Drinks' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Cold Drinks');
INSERT INTO categories (name) SELECT 'Smoothies & Blended' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Smoothies & Blended');
INSERT INTO categories (name) SELECT 'Other' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Other');

-- 30 drinks: (name, sell_price, capital_price, quantity, category_name, reorder_level)
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT t.name, t.sell_price, t.capital_price, t.quantity, NULL, c.id, t.reorder_level
FROM (VALUES
  ('Espresso', 35000, 12000, 100, 'Coffee', 10),
  ('Americano', 38000, 14000, 100, 'Coffee', 10),
  ('Latte', 42000, 16000, 100, 'Coffee', 10),
  ('Cappuccino', 42000, 16000, 100, 'Coffee', 10),
  ('Mocha', 45000, 18000, 100, 'Coffee', 10),
  ('Flat White', 44000, 17000, 100, 'Coffee', 10),
  ('Macchiato', 40000, 15000, 100, 'Coffee', 10),
  ('Cortado', 38000, 14000, 100, 'Coffee', 10),
  ('Cold Brew', 42000, 15000, 100, 'Coffee', 10),
  ('Iced Americano', 40000, 14000, 100, 'Coffee', 10),
  ('Iced Latte', 44000, 16000, 100, 'Coffee', 10),
  ('Iced Mocha', 47000, 18000, 100, 'Coffee', 10),
  ('Iced Coffee', 40000, 14000, 100, 'Cold Drinks', 10),
  ('Matcha Latte', 48000, 20000, 100, 'Tea', 10),
  ('Chai Latte', 45000, 18000, 100, 'Tea', 10),
  ('Green Tea', 35000, 10000, 100, 'Tea', 10),
  ('Black Tea', 35000, 10000, 100, 'Tea', 10),
  ('Iced Green Tea', 38000, 11000, 100, 'Tea', 10),
  ('Thai Tea', 42000, 15000, 100, 'Tea', 10),
  ('Lemonade', 38000, 12000, 100, 'Cold Drinks', 10),
  ('Iced Lemonade', 40000, 12000, 100, 'Cold Drinks', 10),
  ('Orange Juice', 42000, 18000, 100, 'Cold Drinks', 10),
  ('Apple Juice', 40000, 16000, 100, 'Cold Drinks', 10),
  ('Strawberry Smoothie', 52000, 22000, 100, 'Smoothies & Blended', 10),
  ('Mango Smoothie', 52000, 22000, 100, 'Smoothies & Blended', 10),
  ('Banana Smoothie', 48000, 20000, 100, 'Smoothies & Blended', 10),
  ('Chocolate Frappé', 50000, 20000, 100, 'Smoothies & Blended', 10),
  ('Caramel Frappé', 50000, 20000, 100, 'Smoothies & Blended', 10),
  ('Hot Chocolate', 42000, 18000, 100, 'Other', 10),
  ('Fresh Milk', 38000, 15000, 100, 'Other', 10)
) AS t(name, sell_price, capital_price, quantity, category_name, reorder_level)
JOIN categories c ON c.name = t.category_name
WHERE NOT EXISTS (SELECT 1 FROM menu_items m WHERE m.name = t.name);

-- Demo orders (only when orders table is empty; so dashboard and order history show logical data)
INSERT INTO orders (total_price, phone_number, created_at, note, kitchen_status)
SELECT t.total_price, t.phone_number, t.created_at, t.note, t.kitchen_status
FROM (VALUES
  (126000::numeric, '0901234567', CURRENT_TIMESTAMP - INTERVAL '2 days', NULL::text, 'done'),
  (86000::numeric, NULL::text, CURRENT_TIMESTAMP - INTERVAL '1 day', 'Less ice', 'done'),
  (174000::numeric, '0912345678', CURRENT_TIMESTAMP - INTERVAL '5 hours', NULL::text, 'done'),
  (42000::numeric, NULL::text, CURRENT_TIMESTAMP - INTERVAL '1 hour', NULL::text, 'sent'),
  (134000::numeric, '0987654321', CURRENT_TIMESTAMP - INTERVAL '30 minutes', NULL::text, 'pending')
) AS t(total_price, phone_number, created_at, note, kitchen_status)
WHERE NOT EXISTS (SELECT 1 FROM orders);

-- Demo order_details (only when no details exist yet; links to first 5 orders and menu_items by name)
INSERT INTO order_details (order_id, menu_item_id, quantity, price, is_combo)
SELECT ord.oid, m.id, ord.qty, ord.price, false
FROM (VALUES
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 0), 'Latte', 2, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 0), 'Cappuccino', 1, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 0), 'Cold Brew', 1, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 1), 'Americano', 1, 38000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 1), 'Matcha Latte', 1, 48000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 2), 'Latte', 2, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 2), 'Mocha', 1, 45000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 2), 'Chai Latte', 1, 45000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 3), 'Latte', 1, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 4), 'Latte', 2, 42000),
  ((SELECT id FROM orders ORDER BY id ASC LIMIT 1 OFFSET 4), 'Caramel Frappé', 1, 50000)
) AS ord(oid, item_name, qty, price),
menu_items m
WHERE m.name = ord.item_name AND ord.oid IS NOT NULL
  AND EXISTS (SELECT 1 FROM orders) AND NOT EXISTS (SELECT 1 FROM order_details);
