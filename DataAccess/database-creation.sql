CREATE DATABASE IF NOT EXISTS seawave;
USE seawave;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    is_confirmed BOOLEAN DEFAULT FALSE NOT NULL
);

CREATE TABLE tracks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    duration_seconds DOUBLE NOT NULL,
    added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE TABLE playlists (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    user_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE playlists_tracks (
    playlist_id INT NOT NULL,
    track_id INT NOT NULL,
    added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    PRIMARY KEY (playlist_id, track_id),
    FOREIGN KEY (playlist_id) REFERENCES playlists(id) ON DELETE CASCADE,
    FOREIGN KEY (track_id) REFERENCES tracks(id) ON DELETE CASCADE
);

CREATE TABLE upload_queue (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    file_name VARCHAR(500) NOT NULL,
    status ENUM('pending', 'approved', 'rejected') DEFAULT 'pending' NOT NULL,
    request_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE password_reset_tokens (
    user_id INT PRIMARY KEY,
    token_hash VARCHAR(255) NOT NULL,
    expiry TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE user_sessions (
    session_token CHAR(36) PRIMARY KEY,
    user_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE email_verification_tokens (
    user_id INT PRIMARY KEY,
    token_hash CHAR(36) NOT NULL,
    expiry TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

DELIMITER //

CREATE PROCEDURE sp_RegisterUser(IN p_username VARCHAR(50), IN p_email VARCHAR(255), IN p_password_hash VARCHAR(255))
BEGIN 
    INSERT INTO users (username, email, password_hash) VALUES (p_username, p_email, p_password_hash);
    SELECT LAST_INSERT_ID() AS UserId;
END //

CREATE PROCEDURE sp_GetUserById(IN p_user_id INT)
BEGIN 
    SELECT id AS Id, username AS Username, email AS Email, password_hash AS PasswordHash, is_confirmed AS IsConfirmed FROM users
    WHERE id = p_user_id;
END //

CREATE PROCEDURE sp_GetUserByLogin(IN p_identifier VARCHAR(255))
BEGIN
    SELECT id AS Id, username AS Username, email AS Email, password_hash AS PasswordHash, is_confirmed AS IsConfirmed FROM users
    WHERE username = p_identifier OR email = p_identifier;
END //

CREATE PROCEDURE sp_SetPasswordResetToken(IN p_email VARCHAR(255), IN p_token_hash VARCHAR(255), IN p_expiry_minutes INT)
BEGIN 
    DECLARE v_user_id INT;
    SELECT id INTO v_user_id FROM users WHERE email = p_email;
    IF v_user_id IS NOT NULL THEN
        INSERT INTO password_reset_tokens (user_id, token_hash, expiry)
        VALUES (v_user_id, p_token_hash, DATE_ADD(NOW(), INTERVAL p_expiry_minutes MINUTE ))
        ON DUPLICATE KEY UPDATE token_hash = p_token_hash, expiry = DATE_ADD(NOW(), INTERVAL p_expiry_minutes MINUTE);
        SELECT 1 AS Success;
    ELSE
        SELECT 0 AS Success;
    END IF;
END //

CREATE PROCEDURE sp_ResetPasswordWithToken(IN p_token_hash VARCHAR(255), IN p_new_password_hash VARCHAR(255))
BEGIN 
    DECLARE v_user_id INT;
    SELECT user_id INTO v_user_id FROM password_reset_tokens WHERE token_hash = p_token_hash AND expiry > NOW();
    IF v_user_id IS NOT NULL THEN
        UPDATE users SET password_hash = p_new_password_hash WHERE id = v_user_id;
        DELETE FROM password_reset_tokens WHERE user_id = v_user_id;
        SELECT 1 AS Success;
    ELSE
        SELECT 0 AS Success;
    END IF;
END //

CREATE PROCEDURE sp_ChangePassword(IN p_user_id INT, IN p_new_password_hash VARCHAR(255))
BEGIN 
    UPDATE users SET password_hash = p_new_password_hash WHERE id = p_user_id;
END //

CREATE PROCEDURE sp_SearchTracks(IN p_query VARCHAR(255))
BEGIN 
    SELECT id AS Id, title AS Title, artist AS Artist, file_name AS FileName, duration_seconds AS DurationSeconds FROM tracks
    WHERE title LIKE CONCAT('%', p_query, '%') OR artist LIKE CONCAT('%', p_query, '%');
END //

CREATE PROCEDURE sp_SearchPlaylists(IN p_query VARCHAR(255))
BEGIN 
    SELECT playlists.id AS Id, playlists.name AS Name, playlists.user_id AS CreatorId, users.username AS CreatorName,
    (SELECT COUNT(*) FROM playlists_tracks pt WHERE pt.playlist_id = playlists.id) AS TrackCount
    FROM playlists
    JOIN users ON playlists.user_id = users.id
    WHERE playlists.name LIKE CONCAT('%', p_query, '%');
END //

CREATE PROCEDURE sp_GetPlaylistById(IN p_playlist_id INT)
BEGIN
    SELECT playlists.id AS Id, playlists.name AS Name, playlists.user_id AS CreatorId, users.username AS CreatorName
    FROM playlists JOIN users ON playlists.user_id = users.id
    WHERE playlists.id = p_playlist_id;
END //

CREATE PROCEDURE sp_GetPlaylistByUserId(IN p_user_id INT)
BEGIN
    SELECT playlists.id AS Id, playlists.name AS Name, playlists.user_id AS CreatorId, users.username AS CreatorName,
    (SELECT COUNT(*) FROM playlists_tracks pt WHERE pt.playlist_id = playlists.id) AS TrackCount
    FROM playlists
    JOIN users ON playlists.user_id = users.id
    WHERE playlists.user_id = p_user_id;
END //

CREATE PROCEDURE sp_GetPlaylistTracks(IN p_playlist_id INT)
BEGIN 
    SELECT id AS Id, title AS Title, artist AS Artist, file_name AS FileName, duration_seconds AS DurationSeconds FROM tracks
    JOIN playlists_tracks ON tracks.id = playlists_tracks.track_id
    WHERE playlists_tracks.playlist_id = p_playlist_id;
END //

CREATE PROCEDURE sp_CreatePlaylist(IN p_name VARCHAR(100), IN p_user_id INT)
BEGIN 
    INSERT INTO playlists (name, user_id) VALUES (p_name, p_user_id);
    SELECT LAST_INSERT_ID() AS PlaylistId;
END //

CREATE PROCEDURE sp_DeletePlaylist(IN p_user_id INT, IN p_playlist_id INT)
BEGIN 
    DELETE FROM playlists WHERE id = p_playlist_id AND user_id = p_user_id;
    SELECT ROW_COUNT() AS Success;
END //

CREATE PROCEDURE sp_AddTrackToPlaylist(IN p_user_id INT, IN p_playlist_id INT, IN p_track_id INT)
BEGIN 
    DECLARE v_playlist_id INT;
    SELECT id INTO v_playlist_id FROM playlists WHERE user_id = p_user_id;
    IF v_playlist_id IS NOT NULL THEN
        INSERT IGNORE INTO playlists_tracks (playlist_id, track_id) VALUES (p_playlist_id, p_track_id);
        SELECT 1 AS Success;
    ELSE
        SELECT 0 AS Success;
    END IF;
END //

CREATE PROCEDURE sp_RemoveTrackFromPlaylist(IN p_user_id INT, IN p_playlist_id INT, IN p_track_id INT)
BEGIN 
    DELETE pt FROM playlists_tracks pt
    JOIN playlists p ON pt.playlist_id = p.id
    WHERE pt.playlist_id = p_playlist_id AND pt.track_id = p_track_id AND p.user_id = p_user_id;
    SELECT ROW_COUNT() AS Success;
END //

CREATE PROCEDURE sp_RequestUpload(In p_user_id INT, IN p_title VARCHAR(255), IN p_artist VARCHAR(255), IN p_temp_file_name VARCHAR(500))
BEGIN 
    INSERT INTO upload_queue (user_id, title, artist, file_name)
    VALUES (p_user_id, p_title, p_artist, p_temp_file_name);
END //

CREATE PROCEDURE sp_CreateSession(IN p_session_token CHAR(36), IN p_user_id INT, IN p_expiry_days INT)
BEGIN 
    INSERT INTO user_sessions (session_token, user_id, expires_at)
    VALUES (p_session_token, p_user_id, DATE_ADD(NOW(), INTERVAL p_expiry_days DAY));
END //

CREATE PROCEDURE sp_ValidateSession(IN p_session_token CHAR(36))
BEGIN 
    SELECT user_id AS UserId FROM user_sessions
    WHERE session_token = p_session_token AND expires_at > NOW();
END //

CREATE PROCEDURE sp_DeleteSession(IN p_session_token CHAR(36))
BEGIN 
    DELETE FROM user_sessions WHERE session_token = p_session_token;
END //

CREATE PROCEDURE sp_SetEmailVerificationToken(IN p_user_id INT, IN p_token_hash CHAR(36))
BEGIN 
    INSERT INTO email_verification_tokens (user_id, token_hash, expiry)
    VALUES (p_user_id, p_token_hash, DATE_ADD(NOW(), INTERVAL 24 HOUR))
    ON DUPLICATE KEY UPDATE token_hash = p_token_hash, expiry = DATE_ADD(NOW(), INTERVAL 24 HOUR);
END //

CREATE PROCEDURE sp_ConfirmEmail(IN p_token_hash CHAR(36))
BEGIN 
    DECLARE v_user_id INT;
    
    SELECT user_id INTO v_user_id FROM email_verification_tokens
    WHERE token_hash = p_token_hash AND expiry > NOW();
    
    IF v_user_id IS NOT NULL THEN
        UPDATE users SET is_confirmed = TRUE WHERE id = v_user_id;
        DELETE FROM email_verification_tokens WHERE user_id = v_user_id;
        SELECT 1 AS Success;
    ELSE
        SELECT 0 AS Success;
    END IF;
END //

CREATE PROCEDURE sp_GetUserProfileInfo(IN p_user_id INT)
BEGIN 
    SELECT u.username AS Username, u.email AS Email, u.created_at AS CreatedAt,
           (SELECT COUNT(*) FROM playlists p WHERE p.user_id = u.id) AS CreatedPlaylistsCount,
           (SELECT COUNT(*) FROM upload_queue q WHERE q.user_id = u.id AND q.status = 'pending') AS PendingTracksCount,
           (SELECT COUNT(*) FROM upload_queue q WHERE q.user_id = u.id AND q.status = 'approved') AS ApprovedTracksCount
    FROM users u
    WHERE u.id = p_user_id;
END //

DELIMITER ;

CREATE USER IF NOT EXISTS '{DATABASE_USER}'@'localhost' IDENTIFIED BY '{DATABASE_PASSWORD}';

GRANT EXECUTE ON seawave.* TO '{DATABASE_USER}'@'localhost';
REVOKE ALL PRIVILEGES, GRANT OPTION FROM '{DATABASE_USER}'@'localhost';
GRANT EXECUTE ON seawave.* TO '{DATABASE_USER}'@'localhost';

FLUSH PRIVILEGES;