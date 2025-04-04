DROP DATABASE IF EXISTS livin_paris;
CREATE DATABASE livin_paris;
USE livin_paris;

CREATE TABLE users
(
    user_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    password   VARCHAR(255)                       NOT NULL,
    role       SET ('Client', 'Cuisinier')        NOT NULL,
    type       ENUM ('Particulier', 'Entreprise') NOT NULL,
    email      VARCHAR(100) UNIQUE                NOT NULL,
    nom        VARCHAR(50)                        NOT NULL, -- Pour les entreprises contient le nom du contact
    prenom     VARCHAR(50)                        NOT NULL, -- Pour les entreprises contient le prenom du contact
    adresse    VARCHAR(255)                       NOT NULL,
    telephone  VARCHAR(15) UNIQUE                 NOT NULL,
    entreprise VARCHAR(50)                                  -- Pour les entreprises contient le nom de l'entreprise, NULL pour les particuliers

);

CREATE TABLE commandes
(
    commande_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_commande DATETIME,
    adresse_depart TEXT          NOT NULL,
    prix_total     DECIMAL(8, 2) NOT NULL, -- Pourrait etre recalculer
    client_id      BIGINT UNSIGNED,
    cuisinier_id   BIGINT UNSIGNED,

    FOREIGN KEY (client_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id) ON DELETE SET NULL
);

CREATE TABLE recettes
(
    recette_id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_recette        VARCHAR(100)                                 NOT NULL,
    type               ENUM ('Entrée', 'Plat Principal', 'Dessert') NOT NULL,
    ingredients        TEXT                                         NOT NULL,
    style_cuisine      INT                                          NOT NULL, -- ENUM ?
    regime_alimentaire VARCHAR(50),                                           -- SET ? null si pas de regime
    parent_recette_id  BIGINT UNSIGNED UNIQUE,

    FOREIGN KEY (parent_recette_id) REFERENCES recettes (recette_id) ON DELETE SET NULL
);

CREATE TABLE lignes_commandes
(
    ligne_commande_id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_livraison   DATETIME,
    adresse_arrivee   TEXT                                                            NOT NULL,
    statut            ENUM ('Commandee', 'Preparee', 'En cours', 'Livree', 'Annulee') NOT NULL,
    commande_id       BIGINT UNSIGNED                                                 NOT NULL,

    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

CREATE TABLE plats
(
    plat_id           BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_plat          VARCHAR(100)    NOT NULL,
    nb_parts          INT             NOT NULL,
    date_fabrication  DATE            NOT NULL,
    date_peremption   DATE            NOT NULL,
    prix_par_personne DECIMAL(6, 2)   NOT NULL,
    photo             LONGBLOB,
    cuisinier_id      BIGINT UNSIGNED NOT NULL,
    recette_id        BIGINT UNSIGNED NOT NULL,
    commande_id       BIGINT UNSIGNED, -- Null si le plat n'a pas été commandé

    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id),
    FOREIGN KEY (recette_id) REFERENCES recettes (recette_id),
    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

CREATE TABLE evaluation
(
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED,
    note            INT CHECK (note BETWEEN 1 AND 5),
    commentaire     TEXT,
    date_evaluation DATETIME,

    PRIMARY KEY (client_id, cuisinier_id),
    FOREIGN KEY (client_id) REFERENCES users (user_id),
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id)
);

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
