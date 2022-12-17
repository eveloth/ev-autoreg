CREATE TABLE IF NOT EXISTS role (
  id BIGSERIAL PRIMARY KEY, 
  role_name VARCHAR(64) UNIQUE NOT NULL
);
CREATE TABLE IF NOT EXISTS app_user (
  id BIGSERIAL PRIMARY KEY, 
  email VARCHAR(256) UNIQUE NOT NULL, 
  password_hash VARCHAR(60) NOT NULL, 
  first_name VARCHAR(64), 
  last_name VARCHAR(64), 
  is_blocked BOOLEAN NOT NULL DEFAULT false, 
  is_deleted BOOLEAN NOT NULL DEFAULT false, 
  role_id BIGINT REFERENCES role(id) ON DELETE 
  SET 
    NULL
);
CREATE TABLE IF NOT EXISTS permission (
  id BIGSERIAL PRIMARY KEY, 
  permission_name VARCHAR(256) UNIQUE NOT NULL, 
  description VARCHAR(256)
);
CREATE TABLE IF NOT EXISTS role_permission (
  role_id BIGINT REFERENCES role(id) ON DELETE CASCADE, 
  permission_id BIGINT REFERENCES permission(id) ON DELETE CASCADE, 
  PRIMARY KEY (role_id, permission_id)
);
CREATE TABLE IF NOT EXISTS exchange_credentials (
  user_id BIGINT PRIMARY KEY REFERENCES app_user(id) ON DELETE CASCADE, 
  encrypted_email BYTEA NOT NULL, 
  encrypted_password BYTEA NOT NULL, 
  iv BYTEA NOT NULL
);
CREATE TABLE IF NOT EXISTS ev_credentials (
  user_id BIGINT PRIMARY KEY REFERENCES app_user(id) ON DELETE CASCADE, 
  encrypted_email BYTEA NOT NULL, 
  encrypted_password BYTEA NOT NULL, 
  iv BYTEA NOT NULL
);
CREATE TABLE IF NOT EXISTS issue_type (
  id BIGSERIAL PRIMARY KEY, 
  issue_type VARCHAR(32) UNIQUE NOT NULL
);
CREATE TABLE IF NOT EXISTS issue (
  id BIGINT PRIMARY KEY, 
  time_created TIMESTAMP NOT NULL, 
  author VARCHAR(128) NOT NULL, 
  company VARCHAR(128) NOT NULL, 
  status VARCHAR(64) NOT NULL, 
  priority VARCHAR(32) NOT NULL, 
  assigned_group VARCHAR(64) NOT NULL, 
  assignee VARCHAR(128) NOT NULL, 
  short_description TEXT NOT NULL, 
  description TEXT NOT NULL, 
  registrar_id BIGINT NOT NULL REFERENCES app_user(id) ON DELETE 
  SET 
    NULL, 
    issue_type_id BIGINT NOT NULL REFERENCES issue_type(id) ON DELETE 
  SET 
    NULL
);
CREATE TABLE IF NOT EXISTS issue_field (
  id BIGSERIAL PRIMARY KEY, 
  field_name VARCHAR(20) NOT NULL
);
CREATE TABLE IF NOT EXISTS rule (
  id BIGSERIAL PRIMARY KEY, 
  rule VARCHAR(256) NOT NULL, 
  owner_user_id BIGINT NOT NULL REFERENCES app_user(id) ON DELETE CASCADE, 
  issue_type_id BIGINT NOT NULL REFERENCES issue_type(id) ON DELETE CASCADE, 
  Issue_field_id BIGINT NOT NULL REFERENCES issue_field(id) ON DELETE CASCADE, 
  is_regex BOOLEAN NOT NULL DEFAULT false, 
  is_negative BOOLEAN NOT NULL DEFAULT false
);
CREATE TABLE IF NOT EXISTS registering_parameters (
  issue_type_id BIGINT PRIMARY KEY REFERENCES issue_type(id) ON DELETE CASCADE, 
  work_time VARCHAR(4) NOT NULL, 
  reg_status VARCHAR(64) NOT NULL, 
  inwork_status VARCHAR(64), 
  assigned_group VARCHAR(64), 
  request_type VARCHAR(64)
);
CREATE TABLE IF NOT EXISTS autoregistrar_settings (
  id BIGSERIAL PRIMARY KEY, 
  exchange_server_uri VARCHAR(256) NOT NULL, 
  extra_view_uri VARCHAR(256) NOT NULL, 
  new_issue_regex VARCHAR(256) NOT NULL, 
  issue_no_regex VARCHAR(256) NOT NULL
);
