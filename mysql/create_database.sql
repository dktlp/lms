# LMS 0.1.0

CREATE TABLE `artist` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `stage_name` varchar(32) NOT NULL,
  `given_name` varchar(96) NOT NULL,
  `family_name` varchar(96) NOT NULL,
  `address` varchar(96) DEFAULT NULL,
  `city` varchar(32) DEFAULT NULL,
  `district` varchar(32) DEFAULT NULL,
  `state` varchar(32) DEFAULT NULL,
  `postalcode` varchar(16) DEFAULT NULL,
  `country` varchar(32) DEFAULT NULL,
  `email` varchar(128) NOT NULL,
  `telecom` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

CREATE TABLE `tenant` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `key` varchar(32) NOT NULL,
  `email` varchar(128) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

CREATE TABLE `label` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `address` varchar(96) DEFAULT NULL,
  `city` varchar(32) DEFAULT NULL,
  `district` varchar(32) DEFAULT NULL,
  `state` varchar(32) DEFAULT NULL,
  `postalcode` varchar(16) DEFAULT NULL,
  `country` varchar(32) DEFAULT NULL,
  `email` varchar(128) DEFAULT NULL,
  `telecom` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `label_id` int(11) NOT NULL,
  `artist_id` int(11) NOT NULL,
  `name` varchar(32) NOT NULL,
  `status` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

CREATE TABLE `transaction` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `account_id` int(11) NOT NULL,
  `statement_id` int(11) DEFAULT NULL,
  `effective_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `type` tinyint(1) NOT NULL,
  `status` tinyint(1) NOT NULL,
  `amount` double NOT NULL,
  `quarter` varchar(7) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
