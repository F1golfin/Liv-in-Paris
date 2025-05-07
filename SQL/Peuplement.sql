
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise) VALUES
                                                                                                 ('pass123', 'Client', 'Particulier', 'client1@example.com', 'Dupont', 'Alice', '1 rue Paris', '0600000001', NULL),
                                                                                                 ('pass123', 'Cuisinier', 'Particulier', 'cuisinier1@example.com', 'Martin', 'Bob', '2 rue Paris', '0600000002', NULL),

                                                                                                 ('pass123', 'Client', 'Entreprise', 'multi@example.com', 'Durand', 'Claire', '3 rue Paris', '0600000003', 'La Bonne Bouffe');
-- 👥 Insertion de Maxime, Amandine et Guillaume

INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise) VALUES
                                                                                                 ('azerty', 'Client,Cuisinier,Admin', 'Particulier', 'maxime@example.com', 'Rousseau', 'Maxime', '4 rue Paris', '0600000004', NULL),
                                                                                                 ('azerty', 'Client,Cuisinier,Admin', 'Particulier', 'amandine@example.com', 'Baranger', 'Amandine', '5 rue Paris', '0600000005', NULL),
                                                                                                 ('azerty', 'Client,Cuisinier,Admin', 'Particulier', 'guillaume@example.com', 'Blain', 'Guillaume', '6 rue Paris', '0600000006', 'Livin Paris');
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

-- 🍽️ Recettes
INSERT INTO recettes (nom_recette, type, ingredients, style_cuisine, parent_recette_id) VALUES
                                                                                                       ('Salade de quinoa', 'Entrée', 'quinoa, concombre, tomate, citron, huile d’olive', 'Méditerranéenne', NULL), -- Végétarien
                                                                                                       ('Tacos végan', 'Plat Principal', 'galette maïs, haricots noirs, avocat, tofu, épices', 'Mexicaine', NULL), -- Végan
                                                                                                       ('Poulet Tikka Masala', 'Plat Principal', 'poulet, yaourt, épices, tomate, ail, gingembre', 'Indienne', NULL), -- Halal
                                                                                                       ('Gâteau sans gluten', 'Dessert', 'farine de riz, œuf, sucre, chocolat, beurre', 'Française', NULL), -- Sans gluten
                                                                                                       ('Soupe miso', 'Entrée', 'bouillon miso, tofu, algues, oignons verts', 'Japonaise', NULL), -- Végan
                                                                                                       ('Saumon vapeur citron', 'Plat Principal', 'saumon, citron, aneth, sel', 'Nordique', NULL); -- Pescetarien

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
                                                (6, 7); -- Saumon vapeur → Pescetarien



-- 🍽️ Plats associés à des recettes existantes
INSERT INTO plats (nom_plat, nb_parts, date_fabrication, date_peremption, prix_par_personne, cuisinier_id, recette_id)
VALUES
-- Bob Martin
('Bœuf bourguignon', 3, '2025-04-20', '2025-04-23', 9.50, 2, 3), -- recette: Poulet Tikka Masala (similaire au niveau Halal)
('Tarte aux pommes', 4, '2025-04-20', '2025-04-22', 4.00, 2, 4), -- recette sans gluten
('Couscous végétarien', 2, '2025-04-21', '2025-04-24', 8.00, 2, 1), -- recette: Salade de quinoa (Végétarien)
('Salade grecque', 2, '2025-04-21', '2025-04-23', 5.00, 2, 1), -- aussi végétarien

-- Maxime Rousseau
('Wraps au poulet', 3, '2025-04-22', '2025-04-24', 7.50, 4, 3), -- même recette que poulet tikka (base poulet)
('Compote maison', 2, '2025-04-22', '2025-04-23', 3.00, 4, 4), -- recette gâteau sans gluten (sucré)
('Gratin de légumes', 4, '2025-04-21', '2025-04-25', 6.50, 4, 1), -- recette salade quinoa (végé, fibre)
('Riz au lait coco', 3, '2025-04-21', '2025-04-23', 4.50, 4, 5); -- recette soupe miso (vegan)
