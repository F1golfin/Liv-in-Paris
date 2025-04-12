-- 👤 Utilisateurs
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise) VALUES
                                                                                                 ('pass123', 'Client', 'Particulier', 'client1@example.com', 'Dupont', 'Alice', '1 rue Paris', '0600000001', NULL),
                                                                                                 ('pass123', 'Cuisinier', 'Particulier', 'cuisinier1@example.com', 'Martin', 'Bob', '2 rue Paris', '0600000002', NULL),
                                                                                                 ('pass123', 'Client,Cuisinier', 'Entreprise', 'multi@example.com', 'Durand', 'Claire', '3 rue Paris', '0600000003', 'La Bonne Bouffe');
-- 👥 Insertion de Maxime, Amandine et Guillaume
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise) VALUES
                                                                                                 ('azerty', 'Client,Cuisinier', 'Particulier', 'maxime@example.com', 'Rousseau', 'Maxime', '4 rue Paris', '0600000004', NULL),
                                                                                                 ('azerty', 'Client', 'Particulier', 'amandine@example.com', 'Baranger', 'Amandine', '5 rue Paris', '0600000005', NULL),
                                                                                                 ('azerty', 'Client,Cuisinier', 'Entreprise', 'guillaume@example.com', 'Blain', 'Guillaume', '6 rue Paris', '0600000006', 'Livin Paris');
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
INSERT INTO recettes (nom_recette, type, ingredients, style_cuisine, regime_id, parent_recette_id) VALUES
                                                                                                       ('Salade de quinoa', 'Entrée', 'quinoa, concombre, tomate, citron, huile d’olive', 'Méditerranéenne', 1, NULL), -- Végétarien
                                                                                                       ('Tacos végan', 'Plat Principal', 'galette maïs, haricots noirs, avocat, tofu, épices', 'Mexicaine', 2, NULL), -- Végan
                                                                                                       ('Poulet Tikka Masala', 'Plat Principal', 'poulet, yaourt, épices, tomate, ail, gingembre', 'Indienne', 5, NULL), -- Halal
                                                                                                       ('Gâteau sans gluten', 'Dessert', 'farine de riz, œuf, sucre, chocolat, beurre', 'Française', 3, NULL), -- Sans gluten
                                                                                                       ('Soupe miso', 'Entrée', 'bouillon miso, tofu, algues, oignons verts', 'Japonaise', 2, NULL), -- Végan
                                                                                                       ('Saumon vapeur citron', 'Plat Principal', 'saumon, citron, aneth, sel', 'Nordique', 7, NULL); -- Pescetarien

INSERT INTO possede (recette_id, regime_id) VALUES
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


-- 📝 Évaluations fictives
INSERT INTO evaluation (client_id, cuisinier_id, note, commentaire, date_evaluation)
VALUES
    (1, 2, 5, 'Plats très savoureux, bien équilibrés.', NOW()),
    (1, 3, 4, 'Très bon mais un peu trop salé.', NOW()),
    (1, 4, 5, 'Excellent service et cuisine maison délicieuse !', NOW()),
    (3, 2, 4, 'Bonne portion, livraison rapide.', NOW()),
    (5, 3, 3, 'Un peu tiède à l’arrivée mais bon goût.', NOW()),
    (5, 6, 5, 'Parfait pour un déjeuner rapide et sain.', NOW());
