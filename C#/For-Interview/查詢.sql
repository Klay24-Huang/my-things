CREATE DATABASE for_interview;

CREATE TABLE orgs (
	id INT IDENTITY PRIMARY KEY,
	title VARCHAR(30) NOT NULL,
	create_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	update_at DATETIME,
)


CREATE TABLE users (
	id INT IDENTITY PRIMARY KEY,
	org_id INT NOT NULL,
	name VARCHAR(10) NOT NULL,
	birthday DATETIME NOT NULL,
	email VARCHAR(100) NOT NULL UNIQUE,
	account VARCHAR(15) NOT NULL UNIQUE,
	password VARCHAR(100) NOT NULL,
	status BIT NOT NULL  DEFAULT 0,
	create_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	update_at DATETIME,
	FOREIGN KEY (org_id) REFERENCES orgs(id)
)

CREATE TABLE apply_file (
	id INT IDENTITY PRIMARY KEY,
	"user_id" INT NOT NULL,
	file_path VARCHAR(100),
	create_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	update_at DATETIME,
	FOREIGN KEY ("user_id") REFERENCES Users(id)
)

CREATE TABLE syslog (
	seq_no INT IDENTITY PRIMARY KEY,
	account VARCHAR(15) NOT NULL,
	ipaddress VARCHAR(15) NOT NULL,
	login_at DATETIME,
	create_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
)

