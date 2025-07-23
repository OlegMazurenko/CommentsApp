CREATE TABLE `Users` (
  `Id` int PRIMARY KEY AUTO_INCREMENT,
  `UserName` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `HomePage` varchar(255)
);

CREATE TABLE `Comments` (
  `Id` int PRIMARY KEY AUTO_INCREMENT,
  `Text` text NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `ParentCommentId` int,
  `UserId` int NOT NULL
);

CREATE TABLE `FileUploads` (
  `Id` int PRIMARY KEY AUTO_INCREMENT,
  `FileName` varchar(255) NOT NULL,
  `ContentType` varchar(255) NOT NULL,
  `Content` mediumblob NOT NULL,
  `UploadedAt` datetime NOT NULL,
  `CommentId` int NOT NULL
);

CREATE TABLE `Captchas` (
  `Id` char(36) PRIMARY KEY,
  `Code` varchar(10) NOT NULL,
  `Expiration` datetime NOT NULL
);

ALTER TABLE `Comments` ADD FOREIGN KEY (`ParentCommentId`) REFERENCES `Comments` (`Id`);

ALTER TABLE `Comments` ADD FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE;

ALTER TABLE `FileUploads` ADD FOREIGN KEY (`CommentId`) REFERENCES `Comments` (`Id`) ON DELETE CASCADE;
