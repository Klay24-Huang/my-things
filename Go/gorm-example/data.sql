-- CREATE USER 'user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'user';

-- GRANT PRIVILEGE ON database.table TO 'user'@'localhost';

-- GRANT CREATE, ALTER, DROP, INSERT, UPDATE, DELETE, SELECT, REFERENCES, RELOAD on *.* TO 'user'@'localhost' WITH GRANT OPTION;

-- FLUSH PRIVILEGES;

-- CREATE DATABASE test;

CREATE TABLE IF NOT EXISTS employee (
    id int PRIMARY KEY AUTO_INCREMENT,            -- 編號
    name varchar(60) UNIQUE NOT NULL,    -- 名稱
    age integer,                         -- 年齡
    created_at timestamp NOT NULL        -- 建立時間
);

INSERT INTO employee (name, age, created_at) VALUES ('john', 33, now());
INSERT INTO employee (name, age, created_at) VALUES ('mary', 28, NOW()); 

SELECT * FROM employee;