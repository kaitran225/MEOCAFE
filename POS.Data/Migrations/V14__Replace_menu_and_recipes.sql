-- Replace menu with HienXanh menu and add detail recipes for each item.
-- Clears demo orders/details and recipes so menu can be replaced.

DELETE FROM drink_recipes;
DELETE FROM order_details;
DELETE FROM orders;
DELETE FROM menu_item_discount;
DELETE FROM menu_items;
DELETE FROM categories;

-- New categories
INSERT INTO categories (name) VALUES
  ('cacao'),
  ('matcha'),
  ('houjicha'),
  ('caramel'),
  ('fruit tea'),
  ('milk tea'),
  ('espresso'),
  ('cà phê'),
  ('sữa chua'),
  ('khác'),
  ('croffle');

-- Menu items (name, sell_price, capital_price, quantity, category_id, reorder_level)
-- cacao
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cacao latte', 48000, 18000, 100, id, 10 FROM categories WHERE name = 'cacao' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cacao sữa đá / nóng', 45000, 16000, 100, id, 10 FROM categories WHERE name = 'cacao' LIMIT 1;
-- matcha
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'matcha latte', 52000, 20000, 100, id, 10 FROM categories WHERE name = 'matcha' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'coconut matcha cloud', 55000, 22000, 100, id, 10 FROM categories WHERE name = 'matcha' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cold-whisked matcha', 50000, 19000, 100, id, 10 FROM categories WHERE name = 'matcha' LIMIT 1;
-- houjicha
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'houjicha latte', 50000, 19000, 100, id, 10 FROM categories WHERE name = 'houjicha' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'coconut houjicha cloud', 53000, 21000, 100, id, 10 FROM categories WHERE name = 'houjicha' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cold-whisked houjicha', 48000, 18000, 100, id, 10 FROM categories WHERE name = 'houjicha' LIMIT 1;
-- caramel
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'fresh milk caramel', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'caramel' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'caramel macchiato', 50000, 20000, 100, id, 10 FROM categories WHERE name = 'caramel' LIMIT 1;
-- fruit tea
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà đào', 42000, 15000, 100, id, 10 FROM categories WHERE name = 'fruit tea' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà vải hoa hồng', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'fruit tea' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà lài nhãn', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'fruit tea' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà chôm chôm', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'fruit tea' LIMIT 1;
-- milk tea
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà sữa olong hoa lài', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'milk tea' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà sữa gạo rang', 45000, 17000, 100, id, 10 FROM categories WHERE name = 'milk tea' LIMIT 1;
-- espresso
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'espresso / americano', 38000, 14000, 100, id, 10 FROM categories WHERE name = 'espresso' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'yuzu espresso tonic', 52000, 22000, 100, id, 10 FROM categories WHERE name = 'espresso' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'honey coconut espresso', 50000, 20000, 100, id, 10 FROM categories WHERE name = 'espresso' LIMIT 1;
-- cà phê
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cà phê đen', 35000, 12000, 100, id, 10 FROM categories WHERE name = 'cà phê' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cà phê sữa', 38000, 14000, 100, id, 10 FROM categories WHERE name = 'cà phê' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'bạc xỉu', 40000, 15000, 100, id, 10 FROM categories WHERE name = 'cà phê' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'bạc xỉu muối', 42000, 16000, 100, id, 10 FROM categories WHERE name = 'cà phê' LIMIT 1;
-- sữa chua
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'sữa chua dâu', 48000, 20000, 100, id, 10 FROM categories WHERE name = 'sữa chua' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'sữa chua việt quất', 48000, 20000, 100, id, 10 FROM categories WHERE name = 'sữa chua' LIMIT 1;
-- khác
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cam ép', 42000, 18000, 100, id, 10 FROM categories WHERE name = 'khác' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'trà gừng', 35000, 12000, 100, id, 10 FROM categories WHERE name = 'khác' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'nước suối', 15000, 3000, 100, id, 10 FROM categories WHERE name = 'khác' LIMIT 1;
-- croffle
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'biscoff cream', 55000, 22000, 100, id, 10 FROM categories WHERE name = 'croffle' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'cookies and cream', 55000, 22000, 100, id, 10 FROM categories WHERE name = 'croffle' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'matcha cream', 55000, 22000, 100, id, 10 FROM categories WHERE name = 'croffle' LIMIT 1;
INSERT INTO menu_items (name, sell_price, capital_price, quantity, category_id, reorder_level)
SELECT 'mini croffle', 45000, 18000, 100, id, 10 FROM categories WHERE name = 'croffle' LIMIT 1;

-- Drink recipes: ingredients + steps per menu item (default variant_note NULL)
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Cacao powder 15g", "Fresh milk 200ml", "Espresso 1 shot (optional)", "Ice (for đá)"]',
  '["Mix cacao with hot water to paste", "Steam or heat milk", "Combine milk and cacao", "Add ice if đá"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cacao' AND m.name = 'cacao latte';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Cacao powder 12g", "Condensed milk 30ml", "Fresh milk 180ml", "Ice (optional)"]',
  '["Stir cacao with a little hot water", "Add condensed milk", "Add hot milk (nóng) or milk + ice (đá)", "Stir and serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cacao' AND m.name = 'cacao sữa đá / nóng';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Matcha powder 3g", "Hot water 30ml", "Fresh milk 200ml", "Sugar syrup to taste"]',
  '["Whisk matcha with hot water until smooth", "Steam milk", "Pour milk into cup", "Add matcha mix, stir"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'matcha' AND m.name = 'matcha latte';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Matcha powder 4g", "Coconut cream 50ml", "Fresh milk 150ml", "Ice", "Whipped cream"]',
  '["Whisk matcha with cold water", "Blend coconut cream and milk with ice", "Layer matcha and coconut mix", "Top with whipped cream"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'matcha' AND m.name = 'coconut matcha cloud';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Matcha powder 4g", "Cold water 60ml", "Ice", "Milk or water to top"]',
  '["Whisk matcha with cold water in bowl (no lumps)", "Fill glass with ice", "Pour matcha over ice", "Top with milk or water"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'matcha' AND m.name = 'cold-whisked matcha';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Houjicha powder 4g", "Hot water 40ml", "Fresh milk 200ml"]',
  '["Whisk houjicha with hot water", "Steam milk", "Combine and pour"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'houjicha' AND m.name = 'houjicha latte';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Houjicha powder 4g", "Coconut cream 50ml", "Fresh milk 150ml", "Ice", "Whipped cream"]',
  '["Whisk houjicha with cold water", "Blend coconut and milk with ice", "Layer and top with cream"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'houjicha' AND m.name = 'coconut houjicha cloud';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Houjicha powder 4g", "Cold water 60ml", "Ice"]',
  '["Cold-whisk houjicha with water", "Pour over ice", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'houjicha' AND m.name = 'cold-whisked houjicha';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Caramel sauce 30ml", "Fresh milk 220ml", "Ice optional"]',
  '["Drizzle caramel in glass", "Steam or heat milk", "Pour milk, stir", "Add ice if iced"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'caramel' AND m.name = 'fresh milk caramel';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 2 shots", "Caramel sauce 25ml", "Steamed milk 150ml", "Foam"]',
  '["Pull espresso", "Add caramel to cup", "Steam milk", "Pour milk, dot with foam and caramel"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'caramel' AND m.name = 'caramel macchiato';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Peach syrup 30ml", "Jasmine tea 150ml", "Peach pieces", "Ice"]',
  '["Brew jasmine tea, cool", "Add peach syrup and ice", "Add peach pieces", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'fruit tea' AND m.name = 'trà đào';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Lychee rose syrup 30ml", "Tea base 150ml", "Lychee pieces", "Rose petals", "Ice"]',
  '["Brew tea, cool", "Add lychee rose syrup", "Add lychee and rose", "Ice and serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'fruit tea' AND m.name = 'trà vải hoa hồng';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Jasmine tea 150ml", "Longan 30g", "Sugar syrup", "Ice"]',
  '["Brew jasmine tea", "Add longan and syrup", "Cool, add ice", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'fruit tea' AND m.name = 'trà lài nhãn';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Rambutan syrup 30ml", "Tea 150ml", "Rambutan flesh", "Ice"]',
  '["Brew tea, cool", "Add rambutan syrup", "Add rambutan", "Ice and serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'fruit tea' AND m.name = 'trà chôm chôm';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Oolong jasmine tea 150ml", "Fresh milk 80ml", "Tapioca optional", "Sugar syrup"]',
  '["Brew oolong jasmine, cool", "Add sugar syrup", "Add milk and ice", "Add pearls if ordered"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'milk tea' AND m.name = 'trà sữa olong hoa lài';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Roasted rice tea 150ml", "Fresh milk 80ml", "Sugar syrup", "Ice"]',
  '["Steep roasted rice tea", "Cool and strain", "Add milk and syrup", "Serve with ice"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'milk tea' AND m.name = 'trà sữa gạo rang';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 1–2 shots", "Hot water 60–90ml (americano)"]',
  '["Grind 18g, tamp", "Pull double shot", "Add hot water for americano or serve solo"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'espresso' AND m.name = 'espresso / americano';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 2 shots", "Yuzu juice 30ml", "Tonic water 120ml", "Ice"]',
  '["Fill glass with ice", "Add yuzu juice", "Pour tonic", "Top with espresso"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'espresso' AND m.name = 'yuzu espresso tonic';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 2 shots", "Honey 15ml", "Coconut water 100ml", "Ice"]',
  '["Pull espresso", "Stir honey with coconut water", "Ice in glass", "Pour espresso over"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'espresso' AND m.name = 'honey coconut espresso';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Phin brew 25g", "Water 100ml", "Ice optional"]',
  '["Pack phin, add coffee", "Pour hot water", "Drip 5–7 min", "Serve black or over ice"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cà phê' AND m.name = 'cà phê đen';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Phin brew 25g", "Condensed milk 30ml", "Water 80ml", "Ice optional"]',
  '["Brew phin into cup with condensed milk", "Stir", "Serve hot or over ice"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cà phê' AND m.name = 'cà phê sữa';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Phin brew 20g", "Condensed milk 40ml", "Fresh milk 60ml", "Ice"]',
  '["Brew phin stronger", "Mix condensed + fresh milk in glass with ice", "Pour coffee over", "Stir"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cà phê' AND m.name = 'bạc xỉu';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Phin brew 20g", "Condensed milk 35ml", "Fresh milk 60ml", "Salt rim", "Ice"]',
  '["Rim glass with salt", "Build bạc xỉu with ice", "Add salted cream or sprinkle", "Pour coffee"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'cà phê' AND m.name = 'bạc xỉu muối';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Yogurt 150g", "Strawberry purée 50g", "Sugar 15g", "Ice"]',
  '["Blend yogurt with strawberry and sugar", "Add ice", "Blend until smooth", "Pour and serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'sữa chua' AND m.name = 'sữa chua dâu';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Yogurt 150g", "Blueberry 50g", "Sugar 15g", "Ice"]',
  '["Blend yogurt with blueberry and sugar", "Add ice", "Blend until smooth", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'sữa chua' AND m.name = 'sữa chua việt quất';

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Fresh orange 2–3", "Sugar 10g", "Ice"]',
  '["Juice oranges", "Stir in sugar", "Add ice", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'khác' AND m.name = 'cam ép';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Ginger 20g", "Honey 15ml", "Hot water 200ml", "Lemon slice"]',
  '["Slice ginger, simmer in water 5 min", "Strain", "Add honey and lemon", "Serve hot"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'khác' AND m.name = 'trà gừng';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Bottled water 500ml"]',
  '["Chill", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'khác' AND m.name = 'nước suối';

-- Croffle: food items – short prep steps
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Croffle 1 pc", "Biscoff spread 30g", "Whipped cream", "Biscoff crumbs"]',
  '["Toast croffle", "Spread Biscoff", "Top with cream and crumbs", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'croffle' AND m.name = 'biscoff cream';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Croffle 1 pc", "Cookies and cream spread", "Oreo pieces", "Whipped cream"]',
  '["Toast croffle", "Add cookies and cream", "Garnish with Oreo and cream", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'croffle' AND m.name = 'cookies and cream';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Croffle 1 pc", "Matcha cream 40g", "Whipped cream", "Matcha powder dust"]',
  '["Toast croffle", "Pipe matcha cream", "Top with whipped cream", "Dust matcha"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'croffle' AND m.name = 'matcha cream';
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Mini croffle 2 pcs", "Butter", "Maple or honey"]',
  '["Toast mini croffles", "Brush with butter", "Drizzle maple or honey", "Serve"]'
FROM menu_items m JOIN categories c ON m.category_id = c.id WHERE c.name = 'croffle' AND m.name = 'mini croffle';
