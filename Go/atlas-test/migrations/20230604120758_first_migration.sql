-- Create "users" table
CREATE TABLE `users` (`id` int NOT NULL, `name` varchar(200) NULL, `new_name` varchar(200) NULL, PRIMARY KEY (`id`)) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "blog_posts" table
CREATE TABLE `blog_posts` (`id` int NOT NULL, `author_id` int NULL, `title` varchar(100) NULL, `body` text NULL, `comment` varchar(100) NULL, PRIMARY KEY (`id`), INDEX `author_fk` (`author_id`), CONSTRAINT `author_fk` FOREIGN KEY (`author_id`) REFERENCES `users` (`id`) ON UPDATE NO ACTION ON DELETE NO ACTION) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
