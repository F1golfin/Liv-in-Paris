
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise) VALUES
                                                                                                 ('pass123', 'Client', 'Particulier', 'client1@example.com', 'Dupont', 'Alice', '41 Avenue Junot 75018 Paris', '0600000001', NULL),
                                                                                                 ('pass123', 'Cuisinier', 'Particulier', 'cuisinier1@example.com', 'Martin', 'Bob', '87 Rue Haxo 75020 Paris', '0600000002', NULL),
                                                                                                 ('pass123', 'Client', 'Entreprise', 'multi@example.com', 'Durand', 'Claire', '204Z2 Rue Lecourbe 75015 Paris', '0600000003', 'La Bonne Bouffe');

-- Regime alimentaire
INSERT INTO regime_alimentaire (regime)
VALUES
    ('Végétarien'),
    ('Végan'),
    ('Sans gluten'),
    ('Sans lactose'),
    ('Halal'),
    ('Casher'),
    ('Pescetarien'),
    ('Paléo'),
    ('Cétogène'),
    ('Sans sucre'),
    ('Sans œuf'),
    ('Sans arachide'),
    ('Faible en FODMAP'),
    ('Riche en protéines'),
    ('Riche en fibres'),
    ('Diabétique'),
    ('Sans sel'),
    ('Macrobiotique'),
    ('Alimentation intuitive'),
    ('Régime méditerranéen');

-- Recettes
INSERT INTO recettes (nom_recette, type, ingredients, style_cuisine, parent_recette_id) VALUES
                                                                                                       ('Salade de quinoa', 'Entrée', 'quinoa, concombre, tomate, citron, huile d’olive', 'Méditerranéenne', NULL), -- Végétarien
                                                                                                       ('Tacos végan', 'Plat Principal', 'galette maïs, haricots noirs, avocat, tofu, épices', 'Mexicaine', NULL), -- Végan
                                                                                                       ('Poulet Tikka Masala', 'Plat Principal', 'poulet, yaourt, épices, tomate, ail, gingembre', 'Indienne', NULL), -- Halal
                                                                                                       ('Gâteau sans gluten', 'Dessert', 'farine de riz, œuf, sucre, chocolat, beurre', 'Française', NULL), -- Sans gluten
                                                                                                       ('Soupe miso', 'Entrée', 'bouillon miso, tofu, algues, oignons verts', 'Japonaise', NULL), -- Végan
                                                                                                       ('Saumon vapeur citron', 'Plat Principal', 'saumon, citron, aneth, sel', 'Nordique', NULL),
                                                                                                       ('Buddha bowl', 'Plat Principal', 'riz complet, pois chiches, avocat, légumes crus, sauce tahini', 'Fusion', NULL),
                                                                                                       ('Chili sin carne', 'Plat Principal', 'haricots rouges, maïs, poivrons, tomate, oignons', 'Tex-Mex', NULL),
                                                                                                       ('Cheesecake sans lactose', 'Dessert', 'fromage végétal, spéculoos sans beurre, sucre, citron', 'Américaine', NULL),
                                                                                                       ('Taboulé libanais', 'Entrée', 'boulgour, menthe, persil, citron, tomate', 'Libanaise', NULL),
                                                                                                       ('Pâtes sans œuf', 'Plat Principal', 'pâtes de blé dur, sauce tomate maison, basilic', 'Italienne', NULL);

INSERT INTO respecte (recette_id, regime_id) VALUES
                                                (1, 1), -- Salade de quinoa → Végétarien
                                                (1, 14), -- Riche en fibres
                                                (2, 2), -- Tacos végan → Végan
                                                (2, 1), -- aussi végétarien
                                                (3, 5), -- Poulet Tikka Masala → Halal
                                                (4, 3), -- Gâteau sans gluten
                                                (5, 2), -- Soupe miso → Végan
                                                (5, 1), -- aussi végétarien
                                                (5, 14), -- riche en fibres
                                                (6, 7), -- Saumon vapeur → Pescetarien
                                                (7, 1),  -- Buddha bowl → Végétarien
                                                (7, 2),  -- aussi Végan
                                                (7, 14), -- riche en fibres
                                                (8, 1),  -- Chili sin carne → Végétarien
                                                (8, 14),
                                                (9, 4),  -- Cheesecake sans lactose
                                                (9, 10), -- Sans sucre
                                                (10, 1), -- Taboulé → Végétarien
                                                (10, 14), -- riche en fibres
                                                (11, 1), -- Pâtes sans œuf → Végétarien
                                                (11, 11); -- Sans œuf



-- Plats associés à des recettes existantes
INSERT INTO plats (nom_plat, nb_parts, date_fabrication, date_peremption, prix_par_personne, cuisinier_id, recette_id)
VALUES
-- Bob Martin
('Bœuf bourguignon', 3, '2025-04-20', '2025-04-23', 9.50, 5, 3), -- recette: Poulet Tikka Masala (similaire au niveau Halal)
('Tarte aux pommes', 4, '2025-04-20', '2025-04-22', 4.00, 5, 4), -- recette sans gluten
('Couscous végétarien', 2, '2025-04-21', '2025-04-24', 8.00, 5, 1), -- recette: Salade de quinoa (Végétarien)
('Salade grecque', 2, '2025-04-21', '2025-04-23', 5.00, 5, 1), -- aussi végétarien
('Chili végétal épicé', 4, '2025-05-05', '2025-05-08', 7.80, 5, 8),
('Spaghetti tomate maison', 3, '2025-05-05', '2025-05-06', 6.00, 5, 11),

-- Maxime Rousseau
('Wraps au poulet', 3, '2025-04-22', '2025-04-24', 7.50, 1, 3), -- même recette que poulet tikka (base poulet)
('Compote maison', 2, '2025-04-22', '2025-04-23', 3.00, 1, 4), -- recette gâteau sans gluten (sucré)
('Gratin de légumes', 4, '2025-04-21', '2025-04-25', 6.50, 1, 1), -- recette salade quinoa (végé, fibre)
('Riz au lait coco', 3, '2025-04-21', '2025-04-23', 4.50, 1, 5), -- recette soupe miso (vegan)
('Buddha Bowl Zen', 3, '2025-05-05', '2025-05-08', 9.00, 1, 7),
('Cheesecake coco sans lactose', 4, '2025-05-05', '2025-05-07', 5.50, 1, 9),
('Taboulé frais', 5, '2025-05-05', '2025-05-07', 4.20, 1, 10);

                                                                                                                           
