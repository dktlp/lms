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
