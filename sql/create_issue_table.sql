CREATE TABLE if not exists issue (
	id BIGSERIAL NOT NULL PRIMARY KEY,
	issue_no VARCHAR(10) NOT NULL UNIQUE,
	date_created TIMESTAMP NOT NULL,
	author VARCHAR(150) NOT NULL,
	company VARCHAR(150) NOT NULL,
	status VARCHAR(50) NOT NULL,
	priority VARCHAR(50) NOT NULL,
	assigned_group VARCHAR(50) NOT NULL,
	assignee VARCHAR(50) NOT NULL,
	description VARCHAR(300) NOT NULL
);

CREATE OR REPLACE FUNCTION p_upsert_issue(
	p_assigned_group VARCHAR(50),
	p_assignee VARCHAR(50),
	p_author VARCHAR(150),
	p_company VARCHAR(150),
	p_date_created TIMESTAMP,
	p_description VARCHAR(300),
	p_issue_no VARCHAR(10),
	p_priority VARCHAR(50),
	p_status VARCHAR(50))
RETURNS VOID AS $$
BEGIN
	INSERT INTO issue (issue_no, date_created, author, company, status, priority, assigned_group, assignee, description)
	VALUES (p_issue_no, p_date_created, p_author, p_company, p_status, p_priority, p_assigned_group, p_assignee, p_description)
	ON CONFLICT (issue_no)
	DO UPDATE SET
		date_created = EXCLUDED.date_created,
		author = EXCLUDED.author,
		company = EXCLUDED.company,
		status = EXCLUDED.status,
		priority = EXCLUDED.priority,
		assigned_group = EXCLUDED.assigned_group,
		assignee = EXCLUDED.assignee;
END
$$ LANGUAGE plpgsql;

