-- Locations
INSERT INTO location ("Id", "Latitude", "Longitude", "Name") VALUES
(1, 44.7722, 17.1910, 'Banja Luka'),
(2, 43.8563, 18.4131, 'Sarajevo'),
(3, 44.5377, 18.6736, 'Tuzla'),
(4, 43.3438, 17.8078, 'Mostar'),
(5, 45.0000, 19.8333, 'Beograd');

-- Categories
INSERT INTO category ("Id", "Name") VALUES
(1, 'Fire'),
(2, 'Flood'),
(3, 'Traffic'),
(4, 'Crime'),
(5, 'Other');

-- Incidents
INSERT INTO incident ("Id", "Text", "DateTime", "Title", "LocationId") VALUES
(1, 'A fire broke out in the city center near the main square.', '2024-03-01 10:30:00', 'Fire in city center', 1),
(2, 'Heavy flooding reported on the main road leading to the bridge.', '2024-03-02 14:00:00', 'Road flooding', 2),
(3, 'Multiple vehicle accident on the highway near the toll booth.', '2024-03-03 08:15:00', 'Highway accident', 3),
(4, 'Robbery reported at the local bank downtown.', '2024-03-04 16:45:00', 'Bank robbery', 4),
(5, 'Gas leak detected in residential building.', '2024-03-05 09:00:00', 'Gas leak', 5),
(6, 'Suspicious package found near the train station.', '2024-03-06 11:20:00', 'Suspicious package', 1),
(7, 'Forest fire spreading near the national park.', '2024-03-07 13:00:00', 'Forest fire', 2),
(8, 'Bridge closed due to structural damage after flooding.', '2024-03-08 07:30:00', 'Bridge closure', 3);

-- Incident Categories
INSERT INTO incident_category ("IncidentId", "CategoryId") VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 4),
(7, 1),
(8, 2);

-- Images
INSERT INTO images ("Id", "FilePath", "IncidentId") VALUES
(1, '/uploads/incident1_photo1.jpg', 1),
(2, '/uploads/incident2_photo1.jpg', 2),
(3, '/uploads/incident3_photo1.jpg', 3);