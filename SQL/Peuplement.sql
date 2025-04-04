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


-- 🍽️ Recettes
INSERT INTO recettes (nom_recette, type, ingredients, style_cuisine, regime_alimentaire, parent_recette_id) VALUES
                                                                                                                ('Spaghetti Bolognaise', 'Plat Principal', 'Pâtes, viande hachée, tomate', 1, 'Omnivore', NULL),
                                                                                                                ('Soupe Miso', 'Entrée', 'Tofu, algues, miso', 2, 'Vegan', NULL),
                                                                                                                ('Tarte aux pommes', 'Dessert', 'Pomme, pâte brisée, sucre', 1, NULL, NULL);

-- 🍛 Plats (liés aux recettes et cuisiniers)
INSERT INTO plats (nom_plat, nb_parts, date_fabrication, date_peremption, prix_par_personne, cuisinier_id, recette_id) VALUES
                                                                                                                           ('Spaghetti Bolognaise', 5, '2025-04-04', '2025-04-07', 9.50, 2, 1),
                                                                                                                           ('Soupe Miso', 8, '2025-04-04', '2025-04-06', 5.00, 3, 2),
                                                                                                                           ('Tarte aux pommes', 6, '2025-04-04', '2025-04-09', 4.75, 2, 3);

-- 📝 Évaluations fictives
INSERT INTO evaluation (client_id, cuisinier_id, note, commentaire, date_evaluation)
VALUES
    (1, 2, 5, 'Plats très savoureux, bien équilibrés.', NOW()),
    (1, 3, 4, 'Très bon mais un peu trop salé.', NOW()),
    (1, 4, 5, 'Excellent service et cuisine maison délicieuse !', NOW()),
    (3, 2, 4, 'Bonne portion, livraison rapide.', NOW()),
    (5, 3, 3, 'Un peu tiède à l’arrivée mais bon goût.', NOW()),
    (5, 6, 5, 'Parfait pour un déjeuner rapide et sain.', NOW());
