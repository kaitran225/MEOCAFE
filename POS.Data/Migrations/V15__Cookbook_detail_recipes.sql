-- Cookbook-style detail: update drink_recipes with quantities, temperatures, timings, and tips.
-- Each recipe gets rich ingredients (qty, unit, note) and multi-sentence steps.

-- cacao latte
UPDATE drink_recipes SET ingredients = '[
  "15g cacao powder (unsweetened, sifted)",
  "30ml hot water at 85°C (for paste)",
  "200ml fresh full-fat milk (cold)",
  "10–15ml sugar syrup or to taste",
  "1 shot espresso (optional, for mocha-style)",
  "Ice (optional, for iced version)"
]', steps = '[
  "In a small bowl, combine the cacao powder with the 30ml hot water (85°C). Whisk until you have a smooth, lump-free paste. If using espresso, add it now and stir. Set aside.",
  "Steam the milk to 65°C (or heat in a small pot until just below boiling). For iced version, keep milk cold and prepare a glass with ice.",
  "Pour the cacao paste into the serving cup. Add sugar syrup if desired and stir.",
  "Pour the steamed (or cold) milk into the cup in a steady stream, stirring gently to combine. For a latte look, pour milk first then add the cacao mix on top.",
  "Serve immediately. Tip: Do not use boiling water for the paste or the cacao can taste bitter."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cacao latte' LIMIT 1) AND variant_note IS NULL;

-- cacao sữa đá / nóng
UPDATE drink_recipes SET ingredients = '[
  "12g cacao powder (unsweetened)",
  "20–30ml hot water at 80°C",
  "30ml sweetened condensed milk",
  "180ml fresh milk (full-fat preferred)",
  "Ice (for đá version; fill glass)",
  "Pinch of salt (optional)"
]', steps = '[
  "Stir the cacao powder with the hot water in the serving glass or cup until completely dissolved and no lumps remain. Add a pinch of salt if using.",
  "Add the condensed milk and mix well. Adjust sweetness by adding more condensed milk to taste.",
  "For nóng (hot): Heat the 180ml milk to about 70°C and pour into the glass. Stir and serve hot.",
  "For đá (iced): Fill the glass with ice, then pour the cold milk over. Stir and serve with a straw. Tip: Pre-chill the glass for a colder drink."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cacao sữa đá / nóng' LIMIT 1) AND variant_note IS NULL;

-- matcha latte
UPDATE drink_recipes SET ingredients = '[
  "3g matcha powder (ceremonial grade, sifted)",
  "30ml hot water at 80°C (not boiling)",
  "200ml fresh milk (full-fat or oat for richness)",
  "10–20ml sugar syrup or honey to taste",
  "Optional: 1–2 drops vanilla extract"
]', steps = '[
  "Sift the matcha into a small bowl or cup. Heat water to 80°C (boiling water makes matcha bitter). Pour the 30ml hot water over the matcha.",
  "Using a bamboo chasen (or small whisk), whisk in a quick W or M motion until no lumps remain and a fine foam appears on top. This takes about 15–20 seconds.",
  "Steam the milk to 65°C (or heat in a pan). Create a light microfoam if you have a steam wand.",
  "Pour the whisked matcha into the serving cup. Add sugar syrup or honey and stir. Pour the steamed milk over in a steady stream. For latte art, pour milk first then add matcha on top.",
  "Serve immediately. Tip: Store matcha in a cool, dark place and use within a few months of opening for best colour and flavour."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'matcha latte' LIMIT 1) AND variant_note IS NULL;

-- coconut matcha cloud
UPDATE drink_recipes SET ingredients = '[
  "4g matcha powder (sifted)",
  "50ml cold water (for whisking)",
  "50ml coconut cream (chilled)",
  "150ml fresh milk (cold)",
  "15ml sugar syrup or to taste",
  "1 cup ice",
  "Whipped cream for topping (about 30g)",
  "Optional: matcha powder for dusting"
]', steps = '[
  "In a bowl, whisk the matcha with the 50ml cold water until smooth and lump-free. Set aside.",
  "In a blender, combine the coconut cream, milk, sugar syrup, and ice. Blend on high until smooth and frothy (about 20–30 seconds).",
  "Pour the coconut-milk blend into a tall glass, filling about two-thirds. Leave room for the matcha layer.",
  "Slowly pour or spoon the whisked matcha over the coconut layer so it floats on top. You can also layer by pouring over the back of a spoon.",
  "Top with a generous dollop of whipped cream and optionally dust with a little matcha powder. Serve immediately with a straw. Tip: Chill the glass beforehand for a longer-lasting cold drink."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'coconut matcha cloud' LIMIT 1) AND variant_note IS NULL;

-- cold-whisked matcha
UPDATE drink_recipes SET ingredients = '[
  "4g matcha powder (ceremonial or premium, sifted)",
  "60ml cold filtered water",
  "Ice to fill the glass",
  "100–150ml cold milk OR cold water (to top)",
  "10ml sugar syrup (optional)"
]', steps = '[
  "Sift the matcha into a small bowl or chawan. Add the 60ml cold water.",
  "Whisk vigorously with a chasen or small whisk in a W or M motion until completely smooth and no lumps remain (about 20–30 seconds). The mixture will be thick and vibrant green.",
  "Fill a tall glass with ice. If using sugar syrup, add it to the glass now.",
  "Pour the whisked matcha over the ice. Top with cold milk (for a creamier drink) or cold water (for a cleaner, traditional taste) to fill the glass. Stir gently if needed.",
  "Serve immediately. Tip: Cold-whisked matcha avoids any bitterness and highlights umami; use good-quality matcha for best results."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cold-whisked matcha' LIMIT 1) AND variant_note IS NULL;

-- houjicha latte
UPDATE drink_recipes SET ingredients = '[
  "4g houjicha powder (roasted green tea, sifted)",
  "40ml hot water at 90°C",
  "200ml fresh milk (full-fat or plant-based)",
  "10–15ml sugar syrup or honey to taste"
]', steps = '[
  "Sift the houjicha powder into a small bowl. Houjicha is roasted so it can handle slightly hotter water than matcha. Pour the 40ml hot water (90°C) over the powder.",
  "Whisk until smooth and lump-free (about 15 seconds). The mixture will be a deep brown colour with a nutty aroma.",
  "Steam the milk to 65°C (or heat in a small pot). Create a light foam if desired.",
  "Pour the whisked houjicha into the serving cup. Add sugar syrup or honey and stir. Pour the steamed milk over in a steady stream.",
  "Serve immediately. Tip: Houjicha is low in caffeine and has a toasty, comforting flavour; it pairs well with a little vanilla or caramel syrup."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'houjicha latte' LIMIT 1) AND variant_note IS NULL;

-- coconut houjicha cloud
UPDATE drink_recipes SET ingredients = '[
  "4g houjicha powder (sifted)",
  "50ml cold water",
  "50ml coconut cream (chilled)",
  "150ml fresh milk (cold)",
  "15ml sugar syrup",
  "1 cup ice",
  "Whipped cream for topping"
]', steps = '[
  "Whisk the houjicha with the 50ml cold water in a bowl until smooth. Set aside.",
  "In a blender, combine coconut cream, milk, sugar syrup, and ice. Blend on high until frothy (about 20–30 seconds).",
  "Pour the coconut-milk blend into a tall glass, filling about two-thirds.",
  "Layer the whisked houjicha on top (pour slowly or over the back of a spoon). Top with whipped cream.",
  "Serve immediately with a straw. Tip: The roasted flavour of houjicha works well with coconut; adjust sweetness to taste."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'coconut houjicha cloud' LIMIT 1) AND variant_note IS NULL;

-- cold-whisked houjicha
UPDATE drink_recipes SET ingredients = '[
  "4g houjicha powder (sifted)",
  "60ml cold filtered water",
  "Ice to fill glass",
  "100–150ml cold milk or water (to top)",
  "10ml sugar syrup (optional)"
]', steps = '[
  "In a small bowl, whisk the houjicha with the 60ml cold water until smooth and lump-free (about 20 seconds).",
  "Fill a tall glass with ice. Add sugar syrup if using.",
  "Pour the whisked houjicha over the ice. Top with cold milk or water. Stir gently.",
  "Serve immediately. Tip: Cold-whisking houjicha preserves its toasty, nutty flavour without any astringency."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cold-whisked houjicha' LIMIT 1) AND variant_note IS NULL;

-- fresh milk caramel
UPDATE drink_recipes SET ingredients = '[
  "30ml caramel sauce (store-bought or homemade)",
  "220ml fresh full-fat milk (cold for iced, or for steaming)",
  "10ml sugar syrup (optional)",
  "Ice (for iced version; fill glass)",
  "Pinch of sea salt (optional garnish)"
]', steps = '[
  "Drizzle the caramel sauce into the bottom and around the inside of the serving glass. For hot version use a mug; for iced use a tall glass. Add a pinch of salt if desired.",
  "For hot: Steam or heat the milk to 65–70°C. Pour the milk into the glass and stir well to combine with the caramel. Serve hot.",
  "For iced: Fill the glass with ice. Pour the cold milk over and stir until the caramel is dissolved and the drink is well mixed. Serve with a straw.",
  "Tip: Warm the caramel slightly in a microwave (5–10 seconds) if it is too thick to drizzle easily."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'fresh milk caramel' LIMIT 1) AND variant_note IS NULL;

-- caramel macchiato
UPDATE drink_recipes SET ingredients = '[
  "2 shots espresso (about 60ml total)",
  "25ml caramel sauce",
  "150ml fresh milk (for steaming)",
  "10ml vanilla syrup (optional)",
  "Caramel drizzle for top"
]', steps = '[
  "Pull two shots of espresso (about 18g in, 60ml out, 25–30 seconds). Set aside.",
  "Add the caramel sauce (and vanilla syrup if using) to the bottom of the serving cup. This creates the signature ''stained'' look when milk is poured.",
  "Steam the milk to 65°C with a good amount of microfoam. The foam should be velvety and pour smoothly.",
  "Pour the steamed milk into the cup in a steady stream, holding back the foam with the spoon. The espresso will rise and mix with the milk. Dot the top with a spoonful of foam.",
  "Drizzle caramel sauce over the foam in a cross or zigzag pattern. Serve immediately. Tip: Macchiato means ''marked''; the espresso marks the milk, so pour milk first in the cup, then the shot can be added on top for a clearer mark if preferred."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'caramel macchiato' LIMIT 1) AND variant_note IS NULL;

-- trà đào
UPDATE drink_recipes SET ingredients = '[
  "150ml brewed jasmine tea (cooled to room temp or chilled)",
  "30ml peach syrup (or 2–3 tbsp peach purée)",
  "3–4 peach slices (fresh or canned, drained)",
  "10ml sugar syrup or to taste",
  "Ice to fill glass",
  "Mint or basil (optional garnish)"
]', steps = '[
  "Brew jasmine tea: use 2g tea per 150ml water at 85°C, steep 3–4 minutes. Strain and allow to cool completely. Chill in the fridge if serving iced.",
  "Add peach syrup (or purée) and sugar syrup to the cooled tea. Stir well. Taste and adjust sweetness.",
  "Fill a tall glass with ice. Pour the tea mixture over the ice. Add the peach slices (you can muddle one or two for more flavour).",
  "Garnish with mint or basil if desired. Serve with a straw. Tip: For a stronger peach flavour, macerate peach slices in a little sugar and lemon before adding."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà đào' LIMIT 1) AND variant_note IS NULL;

-- trà vải hoa hồng
UPDATE drink_recipes SET ingredients = '[
  "150ml brewed tea base (jasmine or black tea, cooled)",
  "30ml lychee-rose syrup (or 20ml lychee + 10ml rose syrup)",
  "4–5 lychee pieces (canned or fresh, peeled)",
  "Edible dried rose petals or 1 drop rose water",
  "10ml sugar syrup to taste",
  "Ice to fill glass"
]', steps = '[
  "Brew the tea: 2g tea per 150ml water at 90°C, steep 4 minutes. Strain and cool. Chill if serving iced.",
  "Stir the lychee-rose syrup and sugar syrup into the cooled tea. Add a drop of rose water if using (do not overdo; rose can dominate).",
  "Fill a glass with ice. Pour the tea over. Add the lychee pieces and a few rose petals for garnish.",
  "Serve with a straw. Tip: Lychee and rose pair well; balance the two so neither overpowers."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà vải hoa hồng' LIMIT 1) AND variant_note IS NULL;

-- trà lài nhãn
UPDATE drink_recipes SET ingredients = '[
  "150ml brewed jasmine tea (cooled)",
  "30g longan flesh (canned or fresh)",
  "15ml sugar syrup or to taste",
  "Ice to fill glass",
  "1–2 longan for garnish"
]', steps = '[
  "Brew jasmine tea: 2g tea, 150ml water at 85°C, steep 3–4 minutes. Strain and cool.",
  "Add sugar syrup to the cooled tea. Stir. Add half of the longan and gently muddle or leave whole for a clearer look.",
  "Fill glass with ice. Pour the tea over. Add the remaining longan and garnish with a couple of whole longans on top.",
  "Serve with a straw. Tip: Canned longan in syrup can replace some of the sugar syrup; adjust sweetness accordingly."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà lài nhãn' LIMIT 1) AND variant_note IS NULL;

-- trà chôm chôm
UPDATE drink_recipes SET ingredients = '[
  "150ml brewed tea (jasmine or green, cooled)",
  "30ml rambutan syrup (or syrup from canned rambutan)",
  "4–5 rambutan segments (canned or fresh, peeled)",
  "10ml sugar syrup to taste",
  "Ice to fill glass"
]', steps = '[
  "Brew tea: 2g per 150ml, 85°C, 3–4 min. Strain and cool.",
  "Mix rambutan syrup and sugar syrup into the cooled tea. Add a few rambutan segments (optional: muddle one for more flavour).",
  "Fill glass with ice. Pour tea over. Top with remaining rambutan. Serve with a straw.",
  "Tip: Rambutan is similar to lychee; if you don''t have rambutan syrup, lychee syrup is a good substitute."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà chôm chôm' LIMIT 1) AND variant_note IS NULL;

-- trà sữa olong hoa lài
UPDATE drink_recipes SET ingredients = '[
  "150ml brewed oolong jasmine tea (cooled)",
  "80ml fresh milk (cold) or creamer",
  "15–20ml sugar syrup or condensed milk to taste",
  "Tapioca pearls, cooked (optional; 2 tbsp)",
  "Ice to fill glass"
]', steps = '[
  "Brew oolong jasmine: 3g tea, 150ml water at 90°C, steep 4–5 minutes. Strain and cool completely.",
  "Add sugar syrup or condensed milk to the cooled tea. Stir until dissolved. This is your tea base.",
  "If using tapioca: cook pearls according to package (usually boil 20–30 min, rest 20 min), then add 1 tbsp sugar syrup. Add to the bottom of the glass.",
  "Fill glass with ice. Pour the tea base over. Add the cold milk and stir gently (or layer milk on top for a stripe effect). Serve with a wide straw if using pearls. Tip: Oolong jasmine gives a floral, slightly creamy note that pairs well with milk."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà sữa olong hoa lài' LIMIT 1) AND variant_note IS NULL;

-- trà sữa gạo rang
UPDATE drink_recipes SET ingredients = '[
  "150ml roasted rice tea (genmaicha or similar; brewed and cooled)",
  "80ml fresh milk (cold)",
  "15ml sugar syrup or condensed milk to taste",
  "Ice to fill glass"
]', steps = '[
  "Brew roasted rice tea: use 3g per 150ml water at 90°C, steep 3–4 minutes. The tea has a toasty, nutty flavour. Strain and cool.",
  "Add sugar syrup or condensed milk to the cooled tea. Stir well.",
  "Fill a glass with ice. Pour the tea over. Add the cold milk and stir gently.",
  "Serve with a straw. Tip: The roasted rice adds a natural, slightly sweet note; adjust sugar to taste."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà sữa gạo rang' LIMIT 1) AND variant_note IS NULL;

-- espresso / americano
UPDATE drink_recipes SET ingredients = '[
  "18g freshly ground espresso coffee (single-origin or blend)",
  "90–95°C water for extraction",
  "60–90ml hot water (for americano only)"
]', steps = '[
  "Grind 18g coffee to a fine espresso grind (like table salt). Distribute evenly in the portafilter and tamp firmly and evenly (about 15–20 kg pressure). Wipe the rim clean.",
  "Lock the portafilter into the group head. Start the shot immediately. Aim for 25–30 seconds extraction TEXT. For a double shot (60ml out), the flow should be a steady ''mouse tail''.",
  "For espresso: Stop the shot at 25–30ml (single) or 50–60ml (double). Serve in a pre-warmed demitasse.",
  "For americano: Pull the double shot (60ml) into a cup, then add 60–90ml hot water (just off the boil). Stir gently. The result should be strong but drinkable. Tip: Water quality and temperature are critical; use filtered water and consistent brew temperature."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'espresso / americano' LIMIT 1) AND variant_note IS NULL;

-- yuzu espresso tonic
UPDATE drink_recipes SET ingredients = '[
  "2 shots espresso (about 60ml, cooled slightly)",
  "30ml yuzu juice (fresh or bottled)",
  "120ml tonic water (chilled)",
  "10ml sugar syrup (optional)",
  "Ice to fill glass",
  "Slice of yuzu or lemon (garnish)"
]', steps = '[
  "Pull two shots of espresso. Let them cool for 1–2 minutes so they don''t melt the ice too quickly or over-fizz the tonic.",
  "Fill a highball or Collins glass with ice. Pour the yuzu juice and sugar syrup (if using) over the ice. Stir.",
  "Pour the chilled tonic water down the side of the glass to minimise fizz loss. Leave a little space at the top.",
  "Slowly pour the espresso over the back of a spoon onto the tonic layer so it floats and creates a gradient. Garnish with a slice of yuzu or lemon. Serve immediately. Tip: The bittersweet yuzu and quinine in tonic balance the espresso; drink before the ice dilutes too much."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'yuzu espresso tonic' LIMIT 1) AND variant_note IS NULL;

-- honey coconut espresso
UPDATE drink_recipes SET ingredients = '[
  "2 shots espresso (about 60ml)",
  "15ml runny honey",
  "100ml coconut water (chilled)",
  "Ice to fill glass",
  "Optional: 20ml coconut cream for richness"
]', steps = '[
  "Pull two shots of espresso. Set aside to cool slightly.",
  "In the serving glass, add the honey and a splash of coconut water. Stir until the honey is dissolved.",
  "Fill the glass with ice. Pour the remaining chilled coconut water over. Add coconut cream if using and stir gently.",
  "Pour the espresso over the back of a spoon so it layers on top (or stir for a more uniform drink). Serve immediately. Tip: Honey and coconut water add natural sweetness and electrolytes; adjust honey to taste."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'honey coconut espresso' LIMIT 1) AND variant_note IS NULL;

-- cà phê đen
UPDATE drink_recipes SET ingredients = '[
  "25g medium-coarse ground Vietnamese robusta (or robusta/arabica blend)",
  "100ml near-boiling water (93–96°C)",
  "Ice (optional; for cà phê đá)"
]', steps = '[
  "Place the phin filter on top of the cup. Add the ground coffee into the phin (25g). Gently press with the filter screen; do not pack too tightly or the drip will be too slow.",
  "Add 20–30ml hot water to bloom the grounds. Wait 30–45 seconds so the coffee expands and releases gas.",
  "Fill the phin with the remaining hot water (to about 100ml total water). Place the lid on. The coffee will drip through over the next 5–7 minutes. Patience is key for a strong, smooth extraction.",
  "For hot: Serve the concentrated coffee as is, or dilute with a little hot water if preferred. For cà phê đá: Pour the coffee over a glass full of ice and serve. Tip: Vietnamese phin coffee is strong and bold; robusta gives the characteristic bitter-chocolate and earthy notes."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cà phê đen' LIMIT 1) AND variant_note IS NULL;

-- cà phê sữa
UPDATE drink_recipes SET ingredients = '[
  "25g medium-coarse ground Vietnamese coffee",
  "80ml near-boiling water",
  "30ml sweetened condensed milk",
  "Ice (optional; for iced version)"
]', steps = '[
  "Add the condensed milk to the bottom of the serving cup (or glass for iced). This is the classic ''sữa'' base.",
  "Set the phin on top of the cup. Add 25g coffee, press gently, bloom with 20ml water for 30–45 seconds, then fill with the remaining water (about 80ml total). Let drip 5–7 minutes.",
  "When dripping is complete, remove the phin. Stir the coffee and condensed milk together until well mixed and the colour is a rich tan.",
  "For iced: Fill a glass with ice and pour the cà phê sữa over. Serve with a spoon to mix. Tip: The ratio of coffee to condensed milk is to taste; more milk makes it sweeter and milder."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cà phê sữa' LIMIT 1) AND variant_note IS NULL;

-- bạc xỉu
UPDATE drink_recipes SET ingredients = '[
  "20g medium-coarse Vietnamese coffee (slightly more for stronger kick)",
  "70ml near-boiling water",
  "40ml sweetened condensed milk",
  "60ml fresh milk (cold)",
  "Ice to fill glass"
]', steps = '[
  "Brew the coffee using a phin: 20g grounds, bloom 30 sec, then add 70ml water and let drip 5–6 minutes. You want a strong but not overwhelming concentrate.",
  "In a glass, add the condensed milk. Pour the freshly brewed coffee over and stir. The mix should be smooth and tan.",
  "Fill the same glass (or a new one) with ice. Pour the coffee-condensed mix over the ice. Add the cold fresh milk on top and stir gently so the milk clouds into the coffee. The drink is milk-forward and sweeter than cà phê sữa.",
  "Serve with a straw. Tip: Bạc xỉu is often preferred by those who like more milk and sweetness; adjust condensed milk and fresh milk ratio to taste."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'bạc xỉu' LIMIT 1) AND variant_note IS NULL;

-- bạc xỉu muối
UPDATE drink_recipes SET ingredients = '[
  "20g Vietnamese coffee, medium-coarse",
  "70ml near-boiling water",
  "35ml sweetened condensed milk",
  "60ml fresh milk (cold)",
  "2–3g fine salt (for rim or mixed into cream)",
  "Optional: 20ml whipped cream with a pinch of salt",
  "Ice to fill glass"
]', steps = '[
  "Brew coffee in phin: 20g, bloom 30 sec, 70ml water, drip 5–6 min. Mix with 35ml condensed milk in a glass and stir. Set aside.",
  "For salt rim: Run a lime wedge around the rim of the serving glass, then dip the rim in a plate of fine salt. Fill the glass with ice.",
  "Pour the coffee-condensed mixture over the ice. Add the cold fresh milk and stir gently. The salted rim gives a savoury contrast with each sip.",
  "Optional: Top with a layer of lightly salted whipped cream. Serve with a straw. Tip: The salt enhances the sweetness and cuts the richness; don''t over-salt the rim."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'bạc xỉu muối' LIMIT 1) AND variant_note IS NULL;

-- sữa chua dâu
UPDATE drink_recipes SET ingredients = '[
  "150g plain yogurt (full-fat or Greek for thickness)",
  "50g strawberry purée (fresh or frozen, blended)",
  "15g sugar or 15ml sugar syrup",
  "1 cup ice",
  "Optional: 2–3 whole strawberries for garnish"
]', steps = '[
  "In a blender, add the yogurt, strawberry purée, and sugar. Blend on medium until smooth. Taste and add more sugar if needed.",
  "Add the ice and blend on high until the drink is thick and slushy (about 20–30 seconds). If you prefer a smoother drink, blend longer; for more texture, blend less.",
  "Pour into a tall glass. Garnish with a strawberry on the rim or a slice on top. Serve immediately with a straw.",
  "Tip: For a richer flavour, use a mix of fresh and cooked (reduced) strawberry purée; a splash of lemon juice can brighten the taste."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'sữa chua dâu' LIMIT 1) AND variant_note IS NULL;

-- sữa chua việt quất
UPDATE drink_recipes SET ingredients = '[
  "150g plain yogurt (full-fat or Greek)",
  "50g blueberries (fresh or frozen)",
  "15g sugar or 15ml syrup",
  "1 cup ice",
  "Optional: few blueberries for garnish"
]', steps = '[
  "Combine yogurt, blueberries, and sugar in a blender. Blend until smooth (about 15 seconds). Taste and adjust sweetness.",
  "Add the ice and blend on high until thick and slushy (20–30 seconds).",
  "Pour into a glass. Garnish with a few whole blueberries. Serve immediately. Tip: Frozen blueberries make the drink colder and slightly thicker without diluting as much."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'sữa chua việt quất' LIMIT 1) AND variant_note IS NULL;

-- cam ép
UPDATE drink_recipes SET ingredients = '[
  "2–3 large oranges (about 300g total, juiced)",
  "10g sugar or 10ml syrup (optional)",
  "Ice (optional)",
  "Optional: 1 slice orange for garnish"
]', steps = '[
  "Wash the oranges and roll them on the counter to soften. Cut in half and juice with a juicer or reamer. You should get about 120–150ml juice. Strain if you prefer no pulp.",
  "Taste the juice. Add sugar or syrup only if needed (ripe oranges are often sweet enough). Stir until dissolved.",
  "Pour into a glass over ice if desired. Garnish with an orange slice. Serve immediately. Tip: Fresh-squeezed orange juice is best consumed within a few hours; store in the fridge if not serving right away."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cam ép' LIMIT 1) AND variant_note IS NULL;

-- trà gừng
UPDATE drink_recipes SET ingredients = '[
  "20g fresh ginger (sliced or bruised)",
  "200ml water",
  "15ml honey or 15g sugar",
  "1 slice lemon or lime",
  "Optional: 1–2 cloves or a stick of cinnamon"
]', steps = '[
  "Slice or roughly chop the ginger (no need to peel if organic). Bring 200ml water to a boil in a small pot. Add the ginger and optional spices. Simmer over low heat for 5–8 minutes. The longer you simmer, the stronger the ginger flavour.",
  "Remove from heat. Strain into a cup. Add honey or sugar and stir until dissolved. Add a squeeze of lemon or lime.",
  "Serve hot. For iced: Cool the tea, then pour over ice. Tip: Ginger tea soothes the throat; adjust ginger amount for more or less heat."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'trà gừng' LIMIT 1) AND variant_note IS NULL;

-- nước suối
UPDATE drink_recipes SET ingredients = '[
  "500ml bottled or filtered still water (chilled)"
]', steps = '[
  "Chill the water in the fridge or add ice to a glass and pour the water over.",
  "Serve in a glass or bottle. Tip: Room-temperature water is also fine; offer ice on the side if preferred."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'nước suối' LIMIT 1) AND variant_note IS NULL;

-- biscoff cream (croffle)
UPDATE drink_recipes SET ingredients = '[
  "1 croffle (croissant-waffle hybrid, store-bought or pre-made)",
  "30g Biscoff (Lotus) spread",
  "30g whipped cream (chilled, stiff peaks)",
  "1 tbsp Biscoff cookie crumbs",
  "Optional: 1 Biscoff cookie for garnish"
]', steps = '[
  "Preheat a waffle iron or croffle press to medium-high. If the croffle is frozen, thaw slightly. Place the croffle in the press and cook until golden and crisp on both sides (about 2–3 minutes). Timing depends on thickness.",
  "Transfer the cooked croffle to a serving plate. Warm the Biscoff spread in the microwave for 5–10 seconds if it is too thick to spread easily. Spread a generous layer of Biscoff over the top of the croffle.",
  "Pipe or dollop the whipped cream on top of the Biscoff layer. Sprinkle Biscoff cookie crumbs over the cream. Optionally place a whole Biscoff cookie on the side.",
  "Serve immediately while the croffle is still warm. Tip: The contrast of warm croffle, creamy spread, and cold whipped cream is key; don''t let the croffle sit too long before serving."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'biscoff cream' LIMIT 1) AND variant_note IS NULL;

-- cookies and cream (croffle)
UPDATE drink_recipes SET ingredients = '[
  "1 croffle",
  "25g cookies-and-cream spread or cream cheese mixed with crushed Oreo",
  "2–3 Oreo cookies (crushed or as pieces)",
  "30g whipped cream",
  "Optional: 1 Oreo half for garnish"
]', steps = '[
  "Cook the croffle in a waffle iron or croffle press until golden and crisp (about 2–3 minutes). Set on a serving plate.",
  "Spread the cookies-and-cream mixture (or cream cheese–Oreo mix) evenly over the warm croffle. If making the spread: mix 2 tbsp cream cheese with 1 tbsp crushed Oreo and a little sugar to taste.",
  "Top with whipped cream and scatter crushed Oreo pieces or halves over the top. Add an Oreo half on the side if desired.",
  "Serve immediately. Tip: The croffle should be warm so the spread softens slightly; the Oreos add crunch."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'cookies and cream' LIMIT 1) AND variant_note IS NULL;

-- matcha cream (croffle)
UPDATE drink_recipes SET ingredients = '[
  "1 croffle",
  "40g matcha cream (mix 30g whipped cream with 2 tsp matcha powder and 1 tsp sugar)",
  "30g plain whipped cream",
  "A pinch of matcha powder for dusting",
  "Optional: 1 tsp white chocolate chips"
]', steps = '[
  "Cook the croffle in the press until golden and crisp (2–3 minutes). Place on a serving plate.",
  "Prepare matcha cream: sift matcha into a small amount of cream, mix to a paste, then fold into the rest of the whipped cream with sugar. Pipe or spread the matcha cream over the croffle.",
  "Add a layer of plain whipped cream on top or to the side. Dust lightly with matcha powder using a fine sieve. Optionally sprinkle white chocolate chips.",
  "Serve immediately. Tip: Don''t over-whip the matcha cream or it can become grainy; fold matcha in gently."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'matcha cream' LIMIT 1) AND variant_note IS NULL;

-- mini croffle
UPDATE drink_recipes SET ingredients = '[
  "2 mini croffles (or 2 small portions of croffle dough)",
  "10g butter (melted)",
  "15ml maple syrup or honey",
  "Optional: pinch of icing sugar for dusting"
]', steps = '[
  "Preheat the croffle or waffle iron. Place the mini croffles in the press and cook until golden and crisp on both sides (about 1.5–2 minutes each batch).",
  "Brush the cooked mini croffles with melted butter while still hot. This adds shine and flavour.",
  "Drizzle maple syrup or honey over the mini croffles, or serve the syrup on the side for dipping. Optionally dust with a little icing sugar.",
  "Serve warm. Tip: Mini croffles are great for sharing; double the batch and keep them warm in a low oven until ready to serve."
]'
WHERE menu_item_id = (SELECT id FROM menu_items WHERE name = 'mini croffle' LIMIT 1) AND variant_note IS NULL;
