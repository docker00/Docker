--
-- Скрипт сгенерирован Devart dbForge Studio for MySQL, Версия 7.2.78.0
-- Домашняя страница продукта: http://www.devart.com/ru/dbforge/mysql/studio
-- Дата скрипта: 05.09.2017 14:55:51
-- Версия сервера: 5.7.9-log
-- Версия клиента: 4.1
--


-- 
-- Отключение внешних ключей
-- 
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- 
-- Установить режим SQL (SQL mode)
-- 
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 
-- Установка кодировки, с использованием которой клиент будет посылать запросы на сервер
--
SET NAMES 'utf8';

-- 
-- Установка базы данных по умолчанию
--
USE identityserver4_release;

--
-- Описание для таблицы allowed_cors_origins
--
DROP TABLE IF EXISTS allowed_cors_origins;
CREATE TABLE allowed_cors_origins (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  INDEX IDX_allowed_cors_origins_Name (Name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы allowed_grant_types
--
DROP TABLE IF EXISTS allowed_grant_types;
CREATE TABLE allowed_grant_types (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  INDEX IDX_allowed_grant_types_Name (Name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2340
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы claims
--
DROP TABLE IF EXISTS claims;
CREATE TABLE claims (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Type VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id, Type),
  UNIQUE INDEX IDX_claims_Type (Type),
  UNIQUE INDEX UK_claims_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 910
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы clients
--
DROP TABLE IF EXISTS clients;
CREATE TABLE clients (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(255) NOT NULL DEFAULT '',
  ProtocolType VARCHAR(255) NOT NULL DEFAULT 'oidc',
  RequireClientSecret TINYINT(1) NOT NULL DEFAULT 1,
  ClientName VARCHAR(255) NOT NULL DEFAULT '',
  ClientUri VARCHAR(255) NOT NULL DEFAULT '',
  LogoUri VARCHAR(255) NOT NULL DEFAULT '',
  RequireConsent TINYINT(1) NOT NULL DEFAULT 1,
  AllowRememberConsent TINYINT(1) NOT NULL DEFAULT 1,
  RequirePkce TINYINT(1) NOT NULL DEFAULT 0,
  AllowPlainTextPkce TINYINT(1) NOT NULL DEFAULT 0,
  AllowAccessTokensViaBrowser TINYINT(1) NOT NULL DEFAULT 0,
  LogoutUri VARCHAR(255) NOT NULL DEFAULT '',
  LogoutSessionRequired TINYINT(1) NOT NULL DEFAULT 1,
  AllowOfflineAccess TINYINT(1) NOT NULL DEFAULT 0,
  AlwaysIncludeUserClaimsInIdToken TINYINT(1) NOT NULL DEFAULT 0,
  IdentityTokenLifetime INT(11) NOT NULL DEFAULT 300,
  AccessTokenLifetime INT(11) NOT NULL DEFAULT 3600,
  AuthorizationCodeLifetime INT(11) NOT NULL DEFAULT 300,
  AbsoluteRefreshTokenLifetime INT(11) NOT NULL DEFAULT 2592000,
  SlidingRefreshTokenLifetime INT(11) NOT NULL DEFAULT 1296000,
  RefreshTokenUsage INT(11) NOT NULL DEFAULT 1,
  UpdateAccessTokenClaimsOnRefresh TINYINT(1) NOT NULL DEFAULT 0,
  RefreshTokenExpiration INT(11) NOT NULL DEFAULT 1,
  AccessTokenType INT(11) NOT NULL DEFAULT 0,
  EnableLocalLogin TINYINT(1) NOT NULL DEFAULT 1,
  IncludeJwtId TINYINT(1) NOT NULL DEFAULT 0,
  AlwaysSendClientClaims TINYINT(1) NOT NULL DEFAULT 0,
  PrefixClientClaims TINYINT(1) NOT NULL DEFAULT 1,
  Enabled TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_clients_ClientId (ClientId),
  UNIQUE INDEX UK_clients_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы email_smtp_server
--
DROP TABLE IF EXISTS email_smtp_server;
CREATE TABLE email_smtp_server (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  SmtpAdress VARCHAR(128) NOT NULL DEFAULT '',
  SmtpPort INT(11) NOT NULL,
  SslRequred TINYINT(1) NOT NULL DEFAULT 0,
  AuthenticateName VARCHAR(128) NOT NULL DEFAULT '',
  AuthenticateLogin VARCHAR(128) NOT NULL DEFAULT '',
  AuthenticatePassword VARCHAR(128) NOT NULL DEFAULT '',
  Enabled TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX IDX_email_smtp_server (SmtpAdress, SmtpPort),
  INDEX IDX_email_smtp_server_Id (Id),
  UNIQUE INDEX UK_email_smtp_server (SmtpAdress, SmtpPort)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы groups
--
DROP TABLE IF EXISTS groups;
CREATE TABLE groups (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(256) DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX IDX_groups_Name (Name),
  UNIQUE INDEX UK_groups_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы identity_provider_restrictions
--
DROP TABLE IF EXISTS identity_provider_restrictions;
CREATE TABLE identity_provider_restrictions (
  Id VARCHAR(128) NOT NULL,
  Name VARCHAR(255) DEFAULT '',
  PRIMARY KEY (Id),
  INDEX UK_identity_provider_restrict2 (Name),
  UNIQUE INDEX UK_identity_provider_restricti (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы identity_resources
--
DROP TABLE IF EXISTS identity_resources;
CREATE TABLE identity_resources (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  DisplayName VARCHAR(255) NOT NULL DEFAULT '',
  Description VARCHAR(255) NOT NULL DEFAULT '',
  Enabled TINYINT(1) NOT NULL DEFAULT 1,
  Required TINYINT(1) NOT NULL DEFAULT 0,
  Emphasize TINYINT(1) NOT NULL DEFAULT 0,
  ShowInDiscoveryDocument TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_identity_resources_Id (Id),
  UNIQUE INDEX UK_identity_resources_Name (Name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы local_partitions
--
DROP TABLE IF EXISTS local_partitions;
CREATE TABLE local_partitions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ControllerName VARCHAR(255) NOT NULL DEFAULT '',
  ActionName VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  UNIQUE INDEX UK_local_partitions (ControllerName, ActionName)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 212
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы objects
--
DROP TABLE IF EXISTS objects;
CREATE TABLE objects (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  Description VARCHAR(255) NOT NULL DEFAULT '',
  Url VARCHAR(255) DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_objects_Id (Id),
  UNIQUE INDEX UK_objects_Name (Name),
  UNIQUE INDEX UK_objects_Url (Url)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы permissions
--
DROP TABLE IF EXISTS permissions;
CREATE TABLE permissions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id, Name),
  UNIQUE INDEX IDX_permissions_Name (Name),
  UNIQUE INDEX UK_permissions_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы post_logout_redirect_uris
--
DROP TABLE IF EXISTS post_logout_redirect_uris;
CREATE TABLE post_logout_redirect_uris (
  Id VARCHAR(128) NOT NULL,
  Name VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_post_logout_redirect_uris_ (Name),
  UNIQUE INDEX UK_post_logout_redirect_uris_I (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2730
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы profile_attributes
--
DROP TABLE IF EXISTS profile_attributes;
CREATE TABLE profile_attributes (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(256) NOT NULL DEFAULT '',
  RequiredRegister TINYINT(1) NOT NULL DEFAULT 0,
  RequiredAdditional TINYINT(1) NOT NULL DEFAULT 0,
  Disabled TINYINT(1) NOT NULL DEFAULT 0,
  Weight INT(11) NOT NULL DEFAULT 0,
  Deleted TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id, Name),
  UNIQUE INDEX UK_profile_attributes_Id (Id),
  UNIQUE INDEX UK_profile_attributes_Name (Name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы redirect_uris
--
DROP TABLE IF EXISTS redirect_uris;
CREATE TABLE redirect_uris (
  Id VARCHAR(128) NOT NULL,
  Name VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_redirect_uris_Name (Name),
  UNIQUE INDEX UK_redirect_uris_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1260
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы roles
--
DROP TABLE IF EXISTS roles;
CREATE TABLE roles (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(256) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_roles_Id (Id),
  UNIQUE INDEX UK_roles_Name (Name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы secret
--
DROP TABLE IF EXISTS secret;
CREATE TABLE secret (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Value TEXT NOT NULL,
  Type VARCHAR(255) NOT NULL DEFAULT 'SharedSecret',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_secret_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 3276
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы users
--
DROP TABLE IF EXISTS users;
CREATE TABLE users (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Email VARCHAR(255) NOT NULL DEFAULT '',
  EmailConfirmed TINYINT(1) NOT NULL DEFAULT 0,
  PasswordHash VARCHAR(255) NOT NULL DEFAULT '',
  SecurityStamp VARCHAR(255) NOT NULL DEFAULT '',
  PhoneNumber VARCHAR(255) NOT NULL DEFAULT '',
  PhoneNumberConfirmed TINYINT(1) NOT NULL DEFAULT 0,
  TwoFactorEnabled TINYINT(1) NOT NULL DEFAULT 0,
  LockoutEndDateUtc DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  LockoutEnabled TINYINT(1) NOT NULL DEFAULT 0,
  AccessFailedCount INT(11) NOT NULL DEFAULT 0,
  UserName VARCHAR(256) NOT NULL DEFAULT '',
  Activated TINYINT(1) NOT NULL DEFAULT 0,
  AttributesValidated TINYINT(1) NOT NULL DEFAULT 0,
  AttributesIncorrect TINYINT(1) NOT NULL DEFAULT 0,
  LastLoginTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (Id),
  UNIQUE INDEX IDX_users (Email, UserName),
  INDEX IDX_users_Activated (Activated),
  INDEX IDX_users_AttributesIncorrect (AttributesIncorrect),
  INDEX IDX_users_AttributesValidated (AttributesValidated),
  INDEX IDX_users_LastLoginTime (LastLoginTime),
  INDEX IDX_users_TwoFactorEnabled (TwoFactorEnabled),
  INDEX IDX_users_UserName (UserName),
  UNIQUE INDEX UK_users_Email (Email),
  UNIQUE INDEX UK_users_Id (Id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2730
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_allowed_cors_origins
--
DROP TABLE IF EXISTS client_allowed_cors_origins;
CREATE TABLE client_allowed_cors_origins (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  AllowedCorsOriginId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX UK_client_allowed_cors_origin2 (ClientId),
  UNIQUE INDEX UK_client_allowed_cors_origin3 (ClientId, AllowedCorsOriginId),
  UNIQUE INDEX UK_client_allowed_cors_origins (Id),
  CONSTRAINT FK_client_allowed_cors_origin2 FOREIGN KEY (AllowedCorsOriginId)
    REFERENCES allowed_cors_origins(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_allowed_cors_origins FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_allowed_grant_types
--
DROP TABLE IF EXISTS client_allowed_grant_types;
CREATE TABLE client_allowed_grant_types (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  AllowedGrantTypeId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_client_allowed_grant_types_ClientId (ClientId),
  UNIQUE INDEX UK_client_allowed_grant_types (ClientId, AllowedGrantTypeId),
  UNIQUE INDEX UK_client_allowed_grant_types_Id (Id),
  CONSTRAINT FK_client_allowed_grant_types_ FOREIGN KEY (AllowedGrantTypeId)
    REFERENCES allowed_grant_types(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_allowed_grant_types2 FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_allowed_identity_resource_scopes
--
DROP TABLE IF EXISTS client_allowed_identity_resource_scopes;
CREATE TABLE client_allowed_identity_resource_scopes (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  IdentityResourceId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_client_allowed_scopes_Clie (ClientId),
  INDEX IDX_client_allowed_scopes_Iden (IdentityResourceId),
  UNIQUE INDEX UK_client_allowed_identity_res (ClientId, IdentityResourceId),
  UNIQUE INDEX UK_client_allowed_scopes_Id (Id),
  CONSTRAINT FK_client_allowed_scopes_Clien FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_allowed_scopes_Ident FOREIGN KEY (IdentityResourceId)
    REFERENCES identity_resources(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2048
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_claims_todo
--
DROP TABLE IF EXISTS client_claims_todo;
CREATE TABLE client_claims_todo (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  ClaimId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX UK_client_allowed_cors_origin2 (ClientId),
  UNIQUE INDEX UK_client_allowed_cors_origin3 (ClientId, ClaimId),
  UNIQUE INDEX UK_client_allowed_cors_origins (Id),
  CONSTRAINT FK_client_claims_ClientId FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_claims_Id FOREIGN KEY (ClaimId)
    REFERENCES claims(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_identity_provider_restrictions
--
DROP TABLE IF EXISTS client_identity_provider_restrictions;
CREATE TABLE client_identity_provider_restrictions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  IdentityProviderRestrictionId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_client_identity_provider_r2 (ClientId, IdentityProviderRestrictionId),
  INDEX UK_client_identity_provider_r3 (ClientId),
  UNIQUE INDEX UK_client_identity_provider_re (Id),
  CONSTRAINT FK_client_identity_provider_r2 FOREIGN KEY (IdentityProviderRestrictionId)
    REFERENCES identity_provider_restrictions(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_identity_provider_re FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_post_logout_redirect_uris
--
DROP TABLE IF EXISTS client_post_logout_redirect_uris;
CREATE TABLE client_post_logout_redirect_uris (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  PostLogoutRedirectUriId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_client_post_logout_redirec2 (ClientId, PostLogoutRedirectUriId),
  INDEX UK_client_post_logout_redirec3 (ClientId),
  UNIQUE INDEX UK_client_post_logout_redirect (Id),
  CONSTRAINT FK_client_post_logout_redirec2 FOREIGN KEY (PostLogoutRedirectUriId)
    REFERENCES post_logout_redirect_uris(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_post_logout_redirect FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2730
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_redirect_uris
--
DROP TABLE IF EXISTS client_redirect_uris;
CREATE TABLE client_redirect_uris (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  RedirectUriId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_client_redirect_uris_Clien (ClientId),
  UNIQUE INDEX UK_client_redirect_uris (ClientId, RedirectUriId),
  UNIQUE INDEX UK_client_redirect_uris_Id (Id),
  CONSTRAINT FK_client_redirect_uris_Client FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_redirect_uris_Redire FOREIGN KEY (RedirectUriId)
    REFERENCES redirect_uris(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_relations
--
DROP TABLE IF EXISTS client_relations;
CREATE TABLE client_relations (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  FromClientId VARCHAR(128) NOT NULL DEFAULT '',
  ToClientId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_client_relations_FromClien (FromClientId),
  INDEX IDX_client_relations_ToClientI (ToClientId),
  UNIQUE INDEX UK_client_relations (FromClientId, ToClientId),
  UNIQUE INDEX UK_client_relations_Id (Id),
  CONSTRAINT FK_client_relations_FromClien2 FOREIGN KEY (FromClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_relations_ToClientId FOREIGN KEY (ToClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2340
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы client_secrets
--
DROP TABLE IF EXISTS client_secrets;
CREATE TABLE client_secrets (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  SecretId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_client_secrets_ClientId (ClientId),
  UNIQUE INDEX UK_client_secrets (ClientId, SecretId),
  UNIQUE INDEX UK_client_secrets_Id (Id),
  CONSTRAINT FK_client_secrets_ClientId FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_client_secrets_Id FOREIGN KEY (SecretId)
    REFERENCES secret(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы group_parents
--
DROP TABLE IF EXISTS group_parents;
CREATE TABLE group_parents (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  GroupId VARCHAR(128) NOT NULL DEFAULT '',
  ParentGroupId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_group_parents_GroupId (GroupId),
  UNIQUE INDEX UK_group_parents_Id (Id),
  CONSTRAINT FK_group_parents_GroupId FOREIGN KEY (GroupId)
    REFERENCES groups(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_group_parents_ParentGroupId FOREIGN KEY (ParentGroupId)
    REFERENCES groups(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы group_roles
--
DROP TABLE IF EXISTS group_roles;
CREATE TABLE group_roles (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  GroupId VARCHAR(128) NOT NULL DEFAULT '',
  RoleId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_group_roles_GroupId (GroupId),
  UNIQUE INDEX UK_group_roles (GroupId, RoleId),
  UNIQUE INDEX UK_group_roles_Id (Id),
  CONSTRAINT FK_group_roles_GroupId FOREIGN KEY (GroupId)
    REFERENCES groups(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_group_roles_RoleId FOREIGN KEY (RoleId)
    REFERENCES roles(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы group_users
--
DROP TABLE IF EXISTS group_users;
CREATE TABLE group_users (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  GroupId VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_group_roles_GroupId (GroupId),
  UNIQUE INDEX UK_group_roles (GroupId, UserId),
  UNIQUE INDEX UK_group_roles_Id (Id),
  CONSTRAINT FK_group_users_GroupId FOREIGN KEY (GroupId)
    REFERENCES groups(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_group_users_UserId FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 3276
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы identity_resource_claims
--
DROP TABLE IF EXISTS identity_resource_claims;
CREATE TABLE identity_resource_claims (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  IdentityResourceId VARCHAR(128) DEFAULT '',
  ClaimId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_identity_resource_claims_I (IdentityResourceId),
  UNIQUE INDEX UK_identity_resource_claims_I2 (IdentityResourceId, ClaimId),
  UNIQUE INDEX UK_identity_resource_claims_Id (Id),
  CONSTRAINT FK_identity_resource_claims_Cl FOREIGN KEY (ClaimId)
    REFERENCES claims(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_identity_resource_claims_Id FOREIGN KEY (IdentityResourceId)
    REFERENCES identity_resources(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 819
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы object_client
--
DROP TABLE IF EXISTS object_client;
CREATE TABLE object_client (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ObjectId VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id, ObjectId, ClientId),
  INDEX IDX_object_permissions_ObjectI (ObjectId),
  UNIQUE INDEX UK_object_client_ClientId (ClientId),
  UNIQUE INDEX UK_object_permissions (ObjectId, ClientId),
  UNIQUE INDEX UK_object_permissions_Id (Id),
  CONSTRAINT FK_object_client_ClientId FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_object_client_ObjectId FOREIGN KEY (ObjectId)
    REFERENCES objects(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы object_endpoints
--
DROP TABLE IF EXISTS object_endpoints;
CREATE TABLE object_endpoints (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  Value VARCHAR(255) NOT NULL DEFAULT '',
  ObjectId VARCHAR(128) NOT NULL DEFAULT '',
  Description VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_object_partitions_ObjectId (ObjectId),
  INDEX IDX_object_partitions_Value (Value),
  UNIQUE INDEX UK_object_partitions_Id (Id),
  CONSTRAINT FK_object_partitions_ObjectId FOREIGN KEY (ObjectId)
    REFERENCES objects(Id) ON DELETE NO ACTION ON UPDATE RESTRICT
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы profile_attribute_claim
--
DROP TABLE IF EXISTS profile_attribute_claim;
CREATE TABLE profile_attribute_claim (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ProfileAttributeId VARCHAR(128) NOT NULL DEFAULT '',
  ClaimId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_user_profile_attributes (ProfileAttributeId, ClaimId),
  UNIQUE INDEX UK_user_profile_attributes_Id (Id),
  CONSTRAINT FK_claim_profile_attribute_Cla FOREIGN KEY (ClaimId)
    REFERENCES claims(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_claim_profile_attribute_Pro FOREIGN KEY (ProfileAttributeId)
    REFERENCES profile_attributes(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1638
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы role_local_partitions
--
DROP TABLE IF EXISTS role_local_partitions;
CREATE TABLE role_local_partitions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  RoleId VARCHAR(128) NOT NULL DEFAULT '',
  LocalPartitionId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_role_permissions_ObjectEnd (LocalPartitionId),
  INDEX IDX_role_permissions_RoleId (RoleId),
  UNIQUE INDEX UK_role_permissions (RoleId, LocalPartitionId),
  UNIQUE INDEX UK_role_permissions_Id (Id),
  CONSTRAINT FK_role_local_partitions_Local FOREIGN KEY (LocalPartitionId)
    REFERENCES local_partitions(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_role_local_partitions_RoleI FOREIGN KEY (RoleId)
    REFERENCES roles(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_api_keys
--
DROP TABLE IF EXISTS user_api_keys;
CREATE TABLE user_api_keys (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  ClientId VARCHAR(255) NOT NULL DEFAULT '',
  ApiKey VARCHAR(128) NOT NULL DEFAULT '',
  ExperienceTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (Id),
  INDEX IDX_user_api_keys_ClientId (ClientId),
  UNIQUE INDEX IDX_user_api_keys_Id (Id),
  UNIQUE INDEX UK_user_api_keys (UserId, ApiKey),
  UNIQUE INDEX UK_user_api_keys_ApiKey (ApiKey),
  UNIQUE INDEX UK_user_api_keys2 (UserId, ClientId),
  CONSTRAINT FK_user_api_keys_ClientId FOREIGN KEY (ClientId)
    REFERENCES clients(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_user_api_keys_UserId FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_claims
--
DROP TABLE IF EXISTS user_claims;
CREATE TABLE user_claims (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  ClaimId VARCHAR(128) NOT NULL DEFAULT '',
  ClaimValue VARCHAR(255) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_user_claims_UserId (UserId),
  UNIQUE INDEX UK_user_claims (UserId, ClaimId),
  UNIQUE INDEX UK_user_claims_Id (Id),
  CONSTRAINT FK_user_claims_ClaimId FOREIGN KEY (ClaimId)
    REFERENCES claims(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT user_claims FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1092
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_confirm_codes
--
DROP TABLE IF EXISTS user_confirm_codes;
CREATE TABLE user_confirm_codes (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  Code VARCHAR(255) NOT NULL DEFAULT '',
  TimeEnd TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (Id),
  INDEX IDX_user_confirm_codes_UserId (UserId),
  UNIQUE INDEX UK_user_confirm_codes_Id (Id),
  CONSTRAINT FK_user_confirm_codes_UserId FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1638
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_devices
--
DROP TABLE IF EXISTS user_devices;
CREATE TABLE user_devices (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  Name VARCHAR(255) NOT NULL DEFAULT '',
  Type VARCHAR(255) NOT NULL DEFAULT '',
  Confirmed TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  INDEX IDX_user_devices_Id (Id),
  CONSTRAINT FK_user_devices_UserId FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_logins
--
DROP TABLE IF EXISTS user_logins;
CREATE TABLE user_logins (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  LoginProvider VARCHAR(128) NOT NULL DEFAULT '',
  ProviderKey VARCHAR(128) NOT NULL DEFAULT '',
  ProviderDisplayName VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX IDX_user_logins_LoginProvider (LoginProvider, ProviderKey),
  INDEX IDX_user_logins_UserId (UserId),
  UNIQUE INDEX UK_user_logins_Id (Id),
  CONSTRAINT user_logins FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы user_roles
--
DROP TABLE IF EXISTS user_roles;
CREATE TABLE user_roles (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  UserId VARCHAR(128) NOT NULL DEFAULT '',
  RoleId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_user_roles (UserId, RoleId),
  UNIQUE INDEX UK_user_roles_Id (Id),
  CONSTRAINT FK_user_roles_RoleId FOREIGN KEY (RoleId)
    REFERENCES roles(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_user_roles_UserId FOREIGN KEY (UserId)
    REFERENCES users(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 2340
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы object_endpoint_permissions
--
DROP TABLE IF EXISTS object_endpoint_permissions;
CREATE TABLE object_endpoint_permissions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  ObjectEndpointId VARCHAR(128) NOT NULL DEFAULT '',
  PermissionId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  UNIQUE INDEX UK_object_partition_permissio2 (ObjectEndpointId, PermissionId),
  INDEX UK_object_partition_permissio3 (ObjectEndpointId),
  UNIQUE INDEX UK_object_partition_permission (Id),
  CONSTRAINT FK_object_partition_permissio2 FOREIGN KEY (PermissionId)
    REFERENCES permissions(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_object_partition_permission FOREIGN KEY (ObjectEndpointId)
    REFERENCES object_endpoints(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Описание для таблицы role_permissions
--
DROP TABLE IF EXISTS role_permissions;
CREATE TABLE role_permissions (
  Id VARCHAR(128) NOT NULL DEFAULT '',
  RoleId VARCHAR(128) NOT NULL DEFAULT '',
  ObjectEndpointPermissionId VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (Id),
  INDEX IDX_role_permissions_ObjectEnd (ObjectEndpointPermissionId),
  INDEX IDX_role_permissions_RoleId (RoleId),
  UNIQUE INDEX UK_role_permissions (RoleId, ObjectEndpointPermissionId),
  UNIQUE INDEX UK_role_permissions_Id (Id),
  CONSTRAINT FK_role_permissions_ObjectEndp FOREIGN KEY (ObjectEndpointPermissionId)
    REFERENCES object_endpoint_permissions(Id) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT FK_role_permissions_RoleId FOREIGN KEY (RoleId)
    REFERENCES roles(Id) ON DELETE CASCADE ON UPDATE NO ACTION
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

-- 
-- Вывод данных для таблицы allowed_cors_origins
--
INSERT INTO allowed_cors_origins VALUES
('439ee194-49b7-41e1-81dc-5abf656cdd91', 'Ресурс 1'),
('fb5ec610-d9ba-4459-a6a4-b19a24cf8d44', 'Ресурс 2'),
('5ae184ca-0c6d-4ddf-aa14-90aeea62337f', 'Ресурс 3'),
('e1bb9749-2855-4215-b089-fd760b126e4c', 'Ресурс 4');

-- 
-- Вывод данных для таблицы allowed_grant_types
--
INSERT INTO allowed_grant_types VALUES
('7', 'authorization_code'),
('1', 'client_credentials'),
('2', 'custom'),
('3', 'custom.nosubject'),
('5', 'hybrid'),
('6', 'implicit'),
('4', 'password');

-- 
-- Вывод данных для таблицы claims
--
INSERT INTO claims VALUES
('1', 'name'),
('10', 'picture'),
('11', 'website'),
('12', 'gender'),
('13', 'birthdate'),
('14', 'zoneinfo'),
('15', 'locale'),
('16', 'updated_at'),
('17', 'email_verified'),
('18', 'location'),
('19', 'phone_number'),
('2', 'email'),
('20', 'phone_number_verified'),
('21', 'address'),
('22', 'aud'),
('23', 'iss'),
('24', 'nbf'),
('25', 'exp'),
('26', 'iat'),
('27', 'amr'),
('28', 'sid'),
('29', 'acr'),
('3', 'sub'),
('30', 'auth_time'),
('31', 'azp'),
('32', 'at_hash'),
('33', 'c_hash'),
('34', 'nonce'),
('35', 'jti'),
('36', 'events'),
('37', 'client_id'),
('38', 'scope'),
('39', 'id'),
('4', 'family_name'),
('40', 'idp'),
('41', 'role'),
('42', 'reference_token_id'),
('43', 'cnf'),
('5', 'given_name'),
('6', 'middle_name'),
('7', 'nickname'),
('8', 'preferred_username'),
('9', 'profile');

-- 
-- Вывод данных для таблицы clients
--
INSERT INTO clients VALUES
('0524bee3-e5b3-44a9-9563-ded53952f0a0', 'python', 'oidc', 1, 'Python test client', 'http://192.168.0.212:5001/', '', 1, 1, 0, 0, 1, 'http://192.168.0.212:5001/logout', 1, 0, 0, 300, 3600, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 1, 1, 1, 1),
('0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', 'mvc.hybrid', 'oidc', 1, 'MVC Hybrid', 'http://identityserver.io', '', 1, 1, 0, 0, 0, '', 1, 1, 0, 300, 3600, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, 1, 1),
('2c2f9c05-622e-4172-9d64-9ae51918b586', 'Java', 'oidc', 1, 'Java test client', '', '', 1, 1, 0, 0, 0, '', 1, 0, 0, 300, 3600, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, 1, 1),
('389ec486-bc91-4415-a4cd-84656ac06ca4', 'client.jwt', 'oidc', 1, 'client.jwt', 'http://localhost:5000', '', 1, 1, 0, 0, 0, '', 1, 0, 0, 300, 3600, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, 1, 1),
('49b83ac7-1c55-4a01-8352-6b03061667a7', 'php', 'oidc', 1, 'PHP Test', 'http://192.168.0.212:8080/', '', 1, 1, 0, 0, 0, 'http://192.168.0.212:8080/logout', 1, 0, 0, 300, 3600, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, 1, 1),
('ead71a21-14b3-40bd-ac5f-e004ea6c67af', 'client', 'oidc', 1, 'client', 'http://localhost:132', '', 1, 1, 0, 0, 0, '', 1, 0, 0, 300, 2000000000, 300, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, 1, 1);

-- 
-- Вывод данных для таблицы email_smtp_server
--
INSERT INTO email_smtp_server VALUES
('64838682-407d-47e5-a707-e1bbd39458b0', 'smtp.gmail.com', 587, 0, 'Администрация сайта', 'bakrotest1@gmail.com', 'axel123!@#qwe', 1);

-- 
-- Вывод данных для таблицы groups
--
INSERT INTO groups VALUES
('52ef5e79-2eea-4481-9e9c-40e1adde8f3a', 'Администраторы'),
('0f0a02c0-28dd-4c37-83a1-19ce4d512764', 'Модераторы'),
('878d5747-5a24-451c-9376-f5f9b54852b8', 'Пользователи');

-- 
-- Вывод данных для таблицы identity_provider_restrictions
--
INSERT INTO identity_provider_restrictions VALUES
('3ca2c69d-0e74-40b2-a463-8fb11ac6854c', 'Значение 1'),
('23d0ca32-8767-4751-b926-0d499b373e3d', 'Значение 2');

-- 
-- Вывод данных для таблицы identity_resources
--
INSERT INTO identity_resources VALUES
('020ba743-92d7-4f65-9bfd-f3fde582749c', 'location', 'Your location', '', 1, 0, 1, 0),
('4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb', 'custom.profile', '', '', 1, 0, 0, 1),
('69790828-313c-4b0c-8f94-9ff3c518d77c', 'openid', 'Your user identifier', '', 1, 1, 0, 1),
('76aea288-aa7c-4f89-8177-65a6d957339c', 'profile', 'User profile', 'Your user profile information (first name, last name, etc.)', 1, 0, 1, 1),
('d4bf2bb5-96a5-4436-a2e6-0d25e9f12ff7', 'ip', 'Your ip address', '', 0, 0, 1, 0),
('e17554f4-145f-469f-a266-9f59aa65a019', 'email', 'Your email address', '', 1, 0, 1, 1);

-- 
-- Вывод данных для таблицы local_partitions
--
INSERT INTO local_partitions VALUES
('e569f4e1-c05f-40be-bb5f-5b8dd68ac514', 'AllowedGrantTypes', 'Add'),
('9d4bae77-0d4b-4636-8b4d-9db4f8833cea', 'AllowedGrantTypes', 'Delete'),
('3b9e33c4-6bf4-4c2c-b881-1c6e6bec0f09', 'AllowedGrantTypes', 'Edit'),
('8344af6c-1056-4ab0-8edd-046485447a64', 'AllowedGrantTypes', 'List'),
('eee84324-5ada-4f3c-bb47-d1640031bda7', 'Claim', 'Add'),
('8ea49510-3b70-4589-8758-d9aa2457d08c', 'Claim', 'Delete'),
('fab53125-f253-4b6c-812f-49de1f271f01', 'Claim', 'Details'),
('bae054ba-081d-4899-83dd-de8d402d0eda', 'Claim', 'Edit'),
('5ccb1303-3ae6-4b52-813a-0ca06d601b0f', 'Claim', 'List'),
('38779ae7-b3e2-479f-88a0-0e7056c3030e', 'Client', 'Add'),
('639e01a6-be4c-4409-a208-05e96af2aba8', 'Client', 'ClientRelationList'),
('a526eb7d-27d2-4e4a-9a84-814024836e67', 'Client', 'Delete'),
('7f8aa24a-2f92-4328-9a01-bf65b401020b', 'Client', 'DeleteClientRelation'),
('515aba47-7542-4cec-b0ed-2333f7711970', 'Client', 'Details'),
('d82e3257-64ca-4c51-96fb-40e3e9fce075', 'Client', 'List'),
('5da547e9-947e-49c0-a789-63a1bf5f8263', 'Client', '_AddClientRelationPartial'),
('c7d28b49-2164-48b1-89ee-4fc828633c0a', 'Client', '_EditPartial'),
('20', 'EmailSmtpServer', 'Add'),
('21', 'EmailSmtpServer', 'Delete'),
('0a9494e8-9e61-4eb8-8b67-ab3a3530a3f0', 'EmailSmtpServer', 'Details'),
('9754d1a8-fde8-4b96-987f-669b647e819a', 'Group', 'Delete'),
('9b976d95-0811-4e8b-8f1d-28399f2be301', 'Group', 'List'),
('b3406789-45d3-4bb4-8d43-fa49e5ab5f0c', 'Group', '_AddPartial'),
('995b3a2e-01c1-4de4-a5e8-86e84c4ff7d3', 'Group', '_EditPartial'),
('aae0a7df-840e-4c10-a7d5-a62a21260296', 'IdentityProviderRestriction', 'Add'),
('6e7c8e54-703c-436f-9d01-0c0c5caf6fa5', 'IdentityProviderRestriction', 'Delete'),
('a6d2c951-8cfa-4a5b-acb0-9633b498dbab', 'IdentityProviderRestriction', 'Edit'),
('636d20ec-b95f-4b95-8725-e37321c62e11', 'IdentityProviderRestriction', 'List'),
('fa82c6da-85e9-43ab-8743-0c729242aff2', 'IdentityResource', 'Add'),
('f1b6818d-d306-403c-82f5-cfa5edafe96a', 'IdentityResource', 'Delete'),
('d29da653-c793-47fc-8355-caa33f08ba2f', 'IdentityResource', 'Details'),
('de79937f-b660-46d0-b342-e73247d1e90b', 'IdentityResource', 'Edit'),
('4c12e0bd-4bcf-4cc0-ad02-3a9262bfb629', 'IdentityResource', 'List'),
('77e1f28a-8a57-476e-a7b6-12b6aa42a215', 'LocalPartition', 'Add'),
('81157e32-9345-47b6-ba16-895ff6ef693a', 'LocalPartition', 'AddForController'),
('c5b45d3b-2b40-4dd8-b4e7-dbd6635d2868', 'LocalPartition', 'Delete'),
('80656165-372e-4974-b64a-2a400378fd55', 'LocalPartition', 'DeleteForController'),
('8ec97bbd-be26-4a23-9743-bea4504aa77e', 'LocalPartition', 'Details'),
('05cfd2b0-1f9a-48e5-8e37-60b8292c8de5', 'LocalPartition', 'Edit'),
('22', 'LocalPartition', 'List'),
('930cfd23-e821-423e-848b-d40af0657399', 'ManageApi', 'ApiKeyUpdate'),
('a32e89fb-1fda-4d9d-b7b5-4a7c236925bd', 'ManageApi', 'DeleteApiKey'),
('329e4c2c-bc47-4e2d-b9bb-f3cd0bf1114a', 'ManageApi', '_ApiKeyAddPartial'),
('50f7d720-3244-489b-8926-8584548de102', 'ManageApi', '_ApiKeysJoinPartial'),
('78e34768-3aee-4c91-9852-0baba8169cca', 'Object', 'Add'),
('efe36177-15ea-4095-8c5d-67e47e23139c', 'Object', 'Delete'),
('34e09645-37a2-4676-bab0-574bc805beed', 'Object', 'Details'),
('74eaddd0-cb0d-4c76-9fa1-9c8ebefb62c1', 'Object', 'Edit'),
('99f17734-91ac-42d4-8b42-a3e146d1c1cb', 'Object', 'List'),
('6cba36ad-76e7-4b62-b462-768e88529ebb', 'Permission', 'Add'),
('5ba83619-7c96-42aa-a414-1642fc020cd8', 'Permission', 'Delete'),
('0d258970-54a8-46ee-ba02-5a216a6ae550', 'Permission', 'Edit'),
('13a31fa8-a974-4d94-a6d7-ee6563c00521', 'Permission', 'List'),
('8cdc2f03-afb2-4bb0-9b8d-77e42c33a1fc', 'ProfileAttribute', 'Add'),
('ff1549ec-1e7c-4375-bc2e-d20c939c8674', 'ProfileAttribute', 'Delete'),
('ec067a08-d268-47b3-bb1e-be8135744154', 'ProfileAttribute', 'Edit'),
('2728548c-db98-459c-bc21-198d7e4a9c2f', 'ProfileAttribute', 'List'),
('3a0a64b4-de4b-4bc2-9565-4376c26eef9b', 'Role', 'Add'),
('455f045a-1bb2-4e59-b285-191213e20eac', 'Role', 'Data'),
('486c464e-fc7b-4f40-a414-eb1cf086ed2e', 'Role', 'Delete'),
('4ce92ad5-6a32-4372-8f4e-7fbd74017361', 'Role', 'Details'),
('2cfa7acb-c532-4121-98b8-0db93f4c3d11', 'Role', 'Edit'),
('54157f0f-49cc-49e8-9af0-ad8ec99aea88', 'Role', 'List'),
('e77a27ef-f5c4-48fe-8d86-72feb234ca1d', 'Role', 'RoleLocalPartitionList'),
('5e973e3d-161b-46a8-8393-1a9849b88ffe', 'Role', 'RolePermissionList'),
('ff18f2f1-53bc-47bd-8016-72edc40156bb', 'User', 'Add'),
('3dc881fb-5122-4c5d-8511-973cd1c89937', 'User', 'ApiKeyUpdate'),
('ac85a077-aff7-4bc0-ad2d-7d90b75cdd6f', 'User', 'Delete'),
('41fcda70-7e68-47cb-ba07-99ab1d515580', 'User', 'DeleteApiKey'),
('9f0c14e5-3308-4a44-80e2-81497741fbd2', 'User', 'Details'),
('944ada42-f73a-47d8-9c21-a6f9f7c59486', 'User', 'List'),
('12b474e6-d464-4090-a694-24d70f942edc', 'User', 'Merge'),
('8ddd2f9c-70fb-470a-ace5-a72b7c3d2802', 'User', '_ApiKeyAddPartial'),
('f732134d-338d-484c-8e58-f61a344068f0', 'User', '_ApiKeysJoinPartial'),
('79fe3b65-d943-437f-9a43-602fd0981b06', 'User', '_EditPartial');

-- 
-- Вывод данных для таблицы objects
--
INSERT INTO objects VALUES
('3dfa1e98-c4f4-4a1d-a830-0c2090382bbf', 'MVC Hybrid', '', 'http://identityserver.io'),
('79731b18-204a-45c7-a5dd-943b200a2a08', 'Java test client', 'test client', 'http://192.168.0.212:5002'),
('7b3fe03a-9f88-4576-9884-3cab05c67a7c', 'client', 'client', 'http://localhost:132'),
('8745746f-0262-431a-afdf-80afaebb4a6b', 'client.jwt', 'client.jwt', 'http://localhost:5000'),
('96e31bb0-481c-4911-bcdb-187ce6dbf19c', 'Python test client', 'test client', 'http://192.168.0.212:5001'),
('aa01e7dc-0482-40ae-8505-637530f9bcf1', 'test url', 'test url desc', 'http://la.ru.ru.ru'),
('b3ccf41a-0a90-4ff8-b596-0e4155f72aba', 'PHP Test', 'php test for authorize with url', 'http://192.168.0.212'),
('b576442c-3b4e-4ef5-87e6-9677f0be32db', 'test url 2', 'test urle desc 2', 'http://123aaa.ru');

-- 
-- Вывод данных для таблицы permissions
--
INSERT INTO permissions VALUES
('06573305-97db-4fa1-9db4-edf376d85cb1', 'DELETE'),
('c1fc9ae1-c544-4e43-b932-f538cbc28519', 'GET'),
('cb0e2d36-07b2-4cc5-baa2-6400022395ec', 'POST'),
('f43194ab-45a9-49c7-b0e7-1788f7efb6ed', 'PUT');

-- 
-- Вывод данных для таблицы post_logout_redirect_uris
--
INSERT INTO post_logout_redirect_uris VALUES
('ab5a6f73-fe66-4656-8ee5-ee272a00d25a', ' Переадресация 2'),
('5', 'http://192.168.0.51:21402/signout-callback-oidc'),
('3', 'http://localhost:21402/signout-callback-oidc'),
('42df7528-e77f-4ea3-8991-9ce791ca7866', 'http://localhost:21402/signout-callback-oidc'),
('2', 'http://localhost:44077/'),
('1', 'http://localhost:44077/signout-callback-oidc'),
('4', 'http://localhost:7017/index.html'),
('4a166855-beb4-4483-acf4-c0fc7939dd10', 'Переадресация 1'),
('6c3d8051-8f02-4d5e-a278-7c9c59473459', 'Переадресация 3'),
('18a19ecf-1b35-4a9d-85bc-91e02a7bce92', 'Переадресация 4'),
('5c04bb21-9e2d-4a05-af0b-5dcfe602f105', 'Переадресация 5');

-- 
-- Вывод данных для таблицы profile_attributes
--
INSERT INTO profile_attributes VALUES
('1', 'Логин', 1, 0, 0, 0, 0),
('10', 'Страна', 1, 0, 0, 0, 0),
('11', 'Регион', 1, 0, 0, 0, 0),
('12', 'Город', 0, 0, 0, 0, 0),
('13', 'Наименование организации', 0, 0, 0, 0, 0),
('14', 'Должность', 1, 0, 0, 0, 0),
('15', 'Сайт', 1, 0, 0, 0, 0),
('16', 'Аккаунт в соц. Сети', 1, 0, 0, 0, 0),
('17', 'Количество заказов', 1, 0, 0, 0, 0),
('18', 'Предпочтения', 0, 0, 0, 0, 0),
('2', 'E-mail', 1, 0, 0, 0, 0),
('2498e40d-1fcd-4ae9-bee2-47decec4cafc', 'Паспортные данные', 1, 0, 0, 0, 0),
('3', 'Пароль', 1, 0, 0, 0, 0),
('4', 'Область профессио-нальных и бизнес-интересов', 0, 0, 0, 0, 0),
('5', 'ФИО', 0, 0, 0, 0, 0),
('6', 'Род деятельности', 0, 0, 0, 0, 0),
('7', 'Телефон мобильный', 1, 0, 0, 0, 0),
('8', 'Телефон служебный', 1, 0, 0, 0, 0),
('9', 'Телефон домашний', 1, 0, 0, 0, 0);

-- 
-- Вывод данных для таблицы redirect_uris
--
INSERT INTO redirect_uris VALUES
('abab25a5-00bb-442f-a9c0-a9b3a5ce6bc9', 'http://192.168.0.212/simplesaml/module.php/openidconnect/resume.php'),
('0476b895-db18-4086-aa57-742bc820e5b2', 'http://192.168.0.212:5001/api'),
('82ab5ea8-db0e-4442-8221-4e181361ad22', 'http://192.168.0.212:5001/api'),
('b76beba5-62ff-4c2e-aa3a-eeaeaf825663', 'http://192.168.0.212:5001/api'),
('2ac640b3-ddad-4d38-aa06-266b13fb2d32', 'http://192.168.0.212:5001/oidc_callback'),
('77e6c50a-8bfe-4d2b-bc33-810b58881f2c', 'http://192.168.0.212:5001/oidc_callback'),
('91b173f7-555a-494b-a7cc-88021a2c0625', 'http://192.168.0.212:5001/oidc_callback'),
('9', 'http://192.168.0.212:8080/php/redirect_uri'),
('cc9b9af4-4943-4752-afca-325e8a75716a', 'http://192.168.0.212:8080/php/redirect_uri'),
('8a4885fa-6d13-4ca2-bf18-4da82cf27439', 'http://192.168.0.212:8080/php/simplesaml/module.php/openidconnect/resume.php'),
('e64e6615-f3f4-46cc-99c0-2f1f23e838c7', 'http://192.168.0.212:8080/php/simplesaml/module.php/openidconnect/resume.php'),
('0c7fc4f7-965d-4e7f-9070-14ae9d5c76ab', 'http://192.168.0.212:8080/python/api'),
('ad1f4ff7-1510-4a3a-b700-b356123acc34', 'http://192.168.0.212:8080/python/api'),
('c6070019-9d03-4ed4-8b5c-c9bc7755d438', 'http://192.168.0.212:8080/python/api'),
('5a051028-6d55-4a3d-b1f0-d7ab7888ad6c', 'http://192.168.0.212:8080/python/oidc_callback'),
('6b79d5f6-6a0e-49cf-88e5-240f48827feb', 'http://192.168.0.212:8080/python/oidc_callback'),
('ad38defb-a15d-4ac1-a740-6a4871ed277d', 'http://192.168.0.212:8080/python/oidc_callback'),
('045dccab-0480-45ee-8561-426a2ff6670a', 'http://192.168.0.212:8080/python/redirect_uri'),
('a2f18f42-d74b-4e9d-a9e3-3e393f81a75b', 'http://192.168.0.212:8080/python/redirect_uri'),
('e2dcfd8f-173d-47cf-b312-e06c4b97567d', 'http://192.168.0.212:8080/python/redirect_uri'),
('3', 'http://localhost:21402/signin-oidc'),
('6aa06bc8-999e-478b-a280-45bb030c24fd', 'http://localhost:21402/signin-oidc'),
('4', 'http://localhost:28895/index.html'),
('2', 'http://localhost:44077/home/callback'),
('1', 'http://localhost:44077/signin-oidc'),
('6', 'http://localhost:7017/callback.html'),
('5', 'http://localhost:7017/index.html'),
('8', 'http://localhost:7017/popup.html'),
('7', 'http://localhost:7017/silent.html');

-- 
-- Вывод данных для таблицы roles
--
INSERT INTO roles VALUES
('512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'Администратор'),
('53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'Гость'),
('81f5596d-f30c-4a5d-8341-7f1b5df64c54', 'Модератор'),
('81f5596d-f30c-4a5d-8341-7f1b5df64c57', 'Пользователь');

-- 
-- Вывод данных для таблицы secret
--
INSERT INTO secret VALUES
('011ec623-8f80-437e-8f20-d862ec84bf2d', 'By3FX9p3cUql7pK1pWLqvyLc6/KVfM/EMFPxF0WhN+k=', 'SharedSecret'),
('295ade1e-0f1a-4e48-908e-5558eec7e913', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('3953b9fe-b7dc-4c5d-989f-d1f306858126', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('39624dbf-ac39-4d2e-b6ca-a588146d1328', 'hOA44+mm0YxkUXP8QYnRNeBOPWbi5GG9O2QIHaDMIgU=', 'SharedSecret'),
('5718adc7-9c37-4703-804b-7f67176a699e', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('574a53db-1dd5-4ce9-a9d8-581f8c28fc4d', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=3', 'X509CertificateBase64'),
('5947bc2e-784f-46a6-b82b-787f4b0d41d6', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('5fcbc016-670b-4ba2-b410-4c1d3876248a', 'vZ0NODC6rqzLN73ECbGJj7g3S9aWyumW+F+l9R9t0+0=', 'SharedSecret'),
('7726e1e0-c7aa-4c70-aec6-81cc90c2d07e', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('7fa23032-a94e-4442-b125-4f04f1467259', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('8cad8725-5f8e-408a-93c6-68076e16a434', 'MIIDATCCAe2gAwIBAgIQoHUYAquk9rBJcq8W+F0FAzAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjMwMDAwWhcNMjAwMTIwMjMwMDAwWjARMQ8wDQYDVQQDEwZDbGllbnQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDSaY4x1eXqjHF1iXQcF3pbFrIbmNw19w/IdOQxbavmuPbhY7jX0IORu/GQiHjmhqWt8F4G7KGLhXLC1j7rXdDmxXRyVJBZBTEaSYukuX7zGeUXscdpgODLQVay/0hUGz54aDZPAhtBHaYbog+yH10sCXgV1Mxtzx3dGelA6pPwiAmXwFxjJ1HGsS/hdbt+vgXhdlzud3ZSfyI/TJAnFeKxsmbJUyqMfoBl1zFKG4MOvgHhBjekp+r8gYNGknMYu9JDFr1ue0wylaw9UwG8ZXAkYmYbn2wN/CpJl3gJgX42/9g87uLvtVAmz5L+rZQTlS1ibv54ScR2lcRpGQiQav/LAgMBAAGjXDBaMBMGA1UdJQQMMAoGCCsGAQUFBwMCMEMGA1UdAQQ8MDqAENIWANpX5DZ3bX3WvoDfy0GhFDASMRAwDgYDVQQDEwdEZXZSb290ghAsWTt7E82DjU1E1p427Qj2MAkGBSsOAwIdBQADggEBADLje0qbqGVPaZHINLn+WSM2czZk0b5NG80btp7arjgDYoWBIe2TSOkkApTRhLPfmZTsaiI3Ro/64q+Dk3z3Kt7w+grHqu5nYhsn7xQFAQUf3y2KcJnRdIEk0jrLM4vgIzYdXsoC6YO+9QnlkNqcN36Y8IpSVSTda6gRKvGXiAhu42e2Qey/WNMFOL+YzMXGt/nDHL/qRKsuXBOarIb++43DV3YnxGTx22llhOnPpuZ9/gnNY7KLjODaiEciKhaKqt/b57mTEz4jTF4kIg6BP03MUfDXeVlM1Qf1jB43G2QQ19n5lUiqTpmQkcfLfyci2uBZ8BkOhXr3Vk9HIk/xBXQ=', 'X509CertificateBase64'),
('970a45b3-62fb-4021-bb43-4669ed0efc74', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('afb6f9b8-a8b0-4ee0-9e68-8b6cc249b0e7', 'hOA44+mm0YxkUXP8QYnRNeBOPWbi5GG9O2QIHaDMIgU=', 'SharedSecret'),
('c164cbea-c749-46b4-b8fb-90b5517809bb', 'dUAb6pgf40vcmm9BVJ0hwuq99n9cG+D74J8NHcZDt34=', 'SharedSecret'),
('dcef523a-5c87-4235-a01b-202d5cc55dcb', 'vZ0NODC6rqzLN73ECbGJj7g3S9aWyumW+F+l9R9t0+0=', 'SharedSecret'),
('ddee978a-3b47-48aa-b1c2-34f5d744635f', 'T3ZASxHhl8cTnW8IM/CQAu26CqxgmNzWURtjQB7hnwo=', 'SharedSecret'),
('ddf51c41-9d29-4913-9120-dff96ed321f0', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=', 'SharedSecret'),
('e43efd1d-4095-4dde-8764-c7c4830b92d6', 'dUAb6pgf40vcmm9BVJ0hwuq99n9cG+D74J8NHcZDt34=', 'SharedSecret'),
('efcf441a-17be-4de9-ada0-3b00d391f8be', 'T3ZASxHhl8cTnW8IM/CQAu26CqxgmNzWURtjQB7hnwo=', 'SharedSecret');

-- 
-- Вывод данных для таблицы users
--
INSERT INTO users VALUES
('4a386632-3128-45ba-9435-1be9072fe577', 'admin@localhost.ru', 1, 'B323B9C9D2890F7B4D26C1C9BCE13333', '', '', 0, 0, '2017-09-05 14:28:24', 0, 0, 'admin@localhost.ru', 1, 1, 0, '2017-09-05 14:28:25'),
('79b69d3a-677e-43c8-a11c-d472c86e0ab8', 'alexkit1990@gmail.com', 1, '', '', '', 0, 0, '2017-09-05 12:28:40', 0, 0, 'Алексей К', 1, 0, 0, '2017-09-05 12:28:41');

-- 
-- Вывод данных для таблицы client_allowed_cors_origins
--
INSERT INTO client_allowed_cors_origins VALUES
('7f94e6c4-88ab-4ad3-9e1a-1b692093453c', '2c2f9c05-622e-4172-9d64-9ae51918b586', '439ee194-49b7-41e1-81dc-5abf656cdd91'),
('b615b4d9-106a-47a1-b207-cc1b116c9359', '2c2f9c05-622e-4172-9d64-9ae51918b586', '5ae184ca-0c6d-4ddf-aa14-90aeea62337f'),
('83561031-8f96-46bc-ad51-12320a7e41b1', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'e1bb9749-2855-4215-b089-fd760b126e4c'),
('7b019571-dc7a-4145-a4e8-d4cdb42790ff', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'fb5ec610-d9ba-4459-a6a4-b19a24cf8d44');

-- 
-- Вывод данных для таблицы client_allowed_grant_types
--
INSERT INTO client_allowed_grant_types VALUES
('e9a2b2b4-b9e8-4e6a-9f87-6cefc4ec8cc9', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '1'),
('3895644f-9973-4f5f-b6ad-2b9ecf76820d', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '7'),
('f22db8aa-6187-4a04-991c-5ab5cd289630', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '5'),
('41586ef4-f605-40e5-92b4-d312f96ee557', '2c2f9c05-622e-4172-9d64-9ae51918b586', '1'),
('b0da479c-dba6-43e4-8e00-08950bd89708', '2c2f9c05-622e-4172-9d64-9ae51918b586', '2'),
('a8bc2d58-c6cd-49b3-92a0-9fec9af7d840', '2c2f9c05-622e-4172-9d64-9ae51918b586', '3'),
('b31fea9b-5460-4e4b-8a11-c433ced09b19', '2c2f9c05-622e-4172-9d64-9ae51918b586', '7'),
('24eae269-8e8c-43d9-9d51-b9a3b17fa1bd', '389ec486-bc91-4415-a4cd-84656ac06ca4', '1'),
('cb107173-a12c-495d-9a23-a9e8e33698e1', '49b83ac7-1c55-4a01-8352-6b03061667a7', '1'),
('e520bc60-832c-4c6a-9505-5b86119d06e8', '49b83ac7-1c55-4a01-8352-6b03061667a7', '7'),
('68686c33-90f9-4379-9aa9-d9cfc873e57f', 'ead71a21-14b3-40bd-ac5f-e004ea6c67af', '1');

-- 
-- Вывод данных для таблицы client_allowed_identity_resource_scopes
--
INSERT INTO client_allowed_identity_resource_scopes VALUES
('6306dbc7-dee8-4ff6-9db1-3b476175a360', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb'),
('bfd58c0e-845b-4f06-afe4-19ca717ae89b', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '69790828-313c-4b0c-8f94-9ff3c518d77c'),
('1a050485-7416-4b55-951d-513ac63d2a42', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '76aea288-aa7c-4f89-8177-65a6d957339c'),
('db7b7043-c641-4cd7-9171-350c8b8c69f2', '0524bee3-e5b3-44a9-9563-ded53952f0a0', 'e17554f4-145f-469f-a266-9f59aa65a019'),
('4', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb'),
('1', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '69790828-313c-4b0c-8f94-9ff3c518d77c'),
('2', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '76aea288-aa7c-4f89-8177-65a6d957339c'),
('3', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', 'e17554f4-145f-469f-a266-9f59aa65a019'),
('3f794b42-b746-4126-8da8-2672deb36e24', '2c2f9c05-622e-4172-9d64-9ae51918b586', '020ba743-92d7-4f65-9bfd-f3fde582749c'),
('215faf38-df48-476c-8f6e-b305719ec922', '2c2f9c05-622e-4172-9d64-9ae51918b586', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb'),
('0d002a18-62c3-4f3f-abc5-78f6c75a14de', '2c2f9c05-622e-4172-9d64-9ae51918b586', '69790828-313c-4b0c-8f94-9ff3c518d77c'),
('3f765aea-63c0-4bed-93e9-12b9be6f875d', '2c2f9c05-622e-4172-9d64-9ae51918b586', '76aea288-aa7c-4f89-8177-65a6d957339c'),
('ac17b9ec-32eb-4a6d-876a-4d8a98b93697', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'd4bf2bb5-96a5-4436-a2e6-0d25e9f12ff7'),
('a7838b61-2f69-4bdb-8990-497eb0d927fe', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'e17554f4-145f-469f-a266-9f59aa65a019'),
('d8b9d476-babf-457f-86a8-c6fd660637da', '389ec486-bc91-4415-a4cd-84656ac06ca4', 'e17554f4-145f-469f-a266-9f59aa65a019'),
('8', '49b83ac7-1c55-4a01-8352-6b03061667a7', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb'),
('5', '49b83ac7-1c55-4a01-8352-6b03061667a7', '69790828-313c-4b0c-8f94-9ff3c518d77c'),
('6', '49b83ac7-1c55-4a01-8352-6b03061667a7', '76aea288-aa7c-4f89-8177-65a6d957339c'),
('ee320ff1-5933-4c72-91d6-d4706aadc876', '49b83ac7-1c55-4a01-8352-6b03061667a7', 'e17554f4-145f-469f-a266-9f59aa65a019');

-- 
-- Вывод данных для таблицы client_claims_todo
--

-- Таблица identityserver4_release.client_claims_todo не содержит данных

-- 
-- Вывод данных для таблицы client_identity_provider_restrictions
--
INSERT INTO client_identity_provider_restrictions VALUES
('83b1c9f5-2577-47e6-9c1a-fa3e11c78b2d', '2c2f9c05-622e-4172-9d64-9ae51918b586', '23d0ca32-8767-4751-b926-0d499b373e3d'),
('f6c401d7-7561-479a-b388-4346208020cf', '2c2f9c05-622e-4172-9d64-9ae51918b586', '3ca2c69d-0e74-40b2-a463-8fb11ac6854c');

-- 
-- Вывод данных для таблицы client_post_logout_redirect_uris
--
INSERT INTO client_post_logout_redirect_uris VALUES
('a8885f4c-ea18-4e31-bfea-f7bdd932a887', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '42df7528-e77f-4ea3-8991-9ce791ca7866'),
('378ae0ce-31e6-4f8b-8ef8-5c85f04b9400', '2c2f9c05-622e-4172-9d64-9ae51918b586', '18a19ecf-1b35-4a9d-85bc-91e02a7bce92'),
('52ffb3d5-b4e2-40ba-9306-b9c6beff6973', '2c2f9c05-622e-4172-9d64-9ae51918b586', '4a166855-beb4-4483-acf4-c0fc7939dd10'),
('17b0426c-c254-4daf-bd05-5c83bc02fc62', '2c2f9c05-622e-4172-9d64-9ae51918b586', '5c04bb21-9e2d-4a05-af0b-5dcfe602f105'),
('2b68731c-ad21-47d9-a900-2e80d3bc4e1d', '2c2f9c05-622e-4172-9d64-9ae51918b586', '6c3d8051-8f02-4d5e-a278-7c9c59473459'),
('fa165e95-029b-47df-a0ad-645703fa36a6', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'ab5a6f73-fe66-4656-8ee5-ee272a00d25a');

-- 
-- Вывод данных для таблицы client_redirect_uris
--
INSERT INTO client_redirect_uris VALUES
('5bbb4989-077b-48f1-b302-8cc9de34d6cd', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '0c7fc4f7-965d-4e7f-9070-14ae9d5c76ab'),
('049ac3be-79e2-4216-a23a-82b37f3a463c', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '5a051028-6d55-4a3d-b1f0-d7ab7888ad6c'),
('ce7afcd4-d87b-458c-a04a-f7d5ffeab8be', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '77e6c50a-8bfe-4d2b-bc33-810b58881f2c'),
('3c04fd0b-8149-4fc0-ac8b-26ebf33e4798', '0524bee3-e5b3-44a9-9563-ded53952f0a0', 'a2f18f42-d74b-4e9d-a9e3-3e393f81a75b'),
('3b1343b5-57bc-4e37-9d63-ed4bc2cd6327', '0524bee3-e5b3-44a9-9563-ded53952f0a0', 'b76beba5-62ff-4c2e-aa3a-eeaeaf825663'),
('d0c0dc60-5d9e-4639-badd-41c07929146b', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '6aa06bc8-999e-478b-a280-45bb030c24fd'),
('bd1e25d9-4489-4395-9330-280cd2912029', '2c2f9c05-622e-4172-9d64-9ae51918b586', '82ab5ea8-db0e-4442-8221-4e181361ad22'),
('0206b1bd-33aa-4c6d-bdcd-588d8f54347d', '2c2f9c05-622e-4172-9d64-9ae51918b586', '91b173f7-555a-494b-a7cc-88021a2c0625'),
('296edd44-37e7-4369-870b-d8018910f38c', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'ad38defb-a15d-4ac1-a740-6a4871ed277d'),
('57d8c3e4-ff06-4f80-a71e-01c7a4df6a47', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'c6070019-9d03-4ed4-8b5c-c9bc7755d438'),
('7fcc4e38-aa77-4a13-a146-ef669b314147', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'e2dcfd8f-173d-47cf-b312-e06c4b97567d'),
('b71d5cc0-8ee2-46cf-ba54-3fe69c56c004', '49b83ac7-1c55-4a01-8352-6b03061667a7', 'abab25a5-00bb-442f-a9c0-a9b3a5ce6bc9'),
('f745287b-2bc7-4de7-a91d-09bd894220ec', '49b83ac7-1c55-4a01-8352-6b03061667a7', 'cc9b9af4-4943-4752-afca-325e8a75716a'),
('28167519-f696-4ce3-88ad-66f68d6b67de', '49b83ac7-1c55-4a01-8352-6b03061667a7', 'e64e6615-f3f4-46cc-99c0-2f1f23e838c7');

-- 
-- Вывод данных для таблицы client_relations
--
INSERT INTO client_relations VALUES
('51aa4be4-dc1a-4963-9b54-3868c99ef58b', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5'),
('cadd58c8-2adb-4a3f-abf7-ddafbc3f92ab', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '49b83ac7-1c55-4a01-8352-6b03061667a7'),
('fbdc08b5-f0f7-4b82-8049-f8ab60d0f1fb', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '0524bee3-e5b3-44a9-9563-ded53952f0a0'),
('40bf09ce-7866-45c9-8d8e-ea9546efeed0', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '49b83ac7-1c55-4a01-8352-6b03061667a7'),
('98487dee-c669-46a9-b527-28f55a0d694f', '389ec486-bc91-4415-a4cd-84656ac06ca4', '0524bee3-e5b3-44a9-9563-ded53952f0a0'),
('e870466b-0442-4d56-a153-fcb7d248535b', '49b83ac7-1c55-4a01-8352-6b03061667a7', '0524bee3-e5b3-44a9-9563-ded53952f0a0'),
('99ffc070-318e-4ed0-bf8c-a265cc1be637', '49b83ac7-1c55-4a01-8352-6b03061667a7', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5');

-- 
-- Вывод данных для таблицы client_secrets
--
INSERT INTO client_secrets VALUES
('fc27644e-f6d5-4c9c-9340-1ea7435bf0cf', '0524bee3-e5b3-44a9-9563-ded53952f0a0', '295ade1e-0f1a-4e48-908e-5558eec7e913'),
('6f29fa23-a762-4ac7-8919-589ab6fee2a1', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5', '5718adc7-9c37-4703-804b-7f67176a699e'),
('66bbdee0-d68a-40f4-ad3f-8880f0cee39b', '2c2f9c05-622e-4172-9d64-9ae51918b586', '011ec623-8f80-437e-8f20-d862ec84bf2d'),
('0567fffa-7695-4a4f-96d9-790c6a6ba97d', '2c2f9c05-622e-4172-9d64-9ae51918b586', '5fcbc016-670b-4ba2-b410-4c1d3876248a'),
('00622c62-03b8-4eae-a591-57b67f80cc5a', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'afb6f9b8-a8b0-4ee0-9e68-8b6cc249b0e7'),
('27e5c432-fc42-4a82-9181-b212656892ef', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'e43efd1d-4095-4dde-8764-c7c4830b92d6'),
('1b8f7cf5-d0f5-41cf-8a2b-9ea0da3c931f', '2c2f9c05-622e-4172-9d64-9ae51918b586', 'efcf441a-17be-4de9-ada0-3b00d391f8be'),
('50706f4c-55a3-44a9-821c-0de54f749313', '389ec486-bc91-4415-a4cd-84656ac06ca4', '8cad8725-5f8e-408a-93c6-68076e16a434'),
('18c18722-6a36-4eee-b5e5-f351672b1606', '49b83ac7-1c55-4a01-8352-6b03061667a7', 'ddf51c41-9d29-4913-9120-dff96ed321f0'),
('a9c53e66-5da8-4e2c-835c-8883fe2ff3d2', 'ead71a21-14b3-40bd-ac5f-e004ea6c67af', '970a45b3-62fb-4021-bb43-4669ed0efc74');

-- 
-- Вывод данных для таблицы group_parents
--

-- Таблица identityserver4_release.group_parents не содержит данных

-- 
-- Вывод данных для таблицы group_roles
--
INSERT INTO group_roles VALUES
('bda6df1d-4237-4e0c-b2a6-603a7b4204b0', '0f0a02c0-28dd-4c37-83a1-19ce4d512764', '53a66484-018a-49f7-9e89-98e8ae3fa0c5'),
('fa9b21b3-07e8-403f-baab-27d095abdbd4', '0f0a02c0-28dd-4c37-83a1-19ce4d512764', '81f5596d-f30c-4a5d-8341-7f1b5df64c54'),
('8a237e6f-0b5c-4942-989d-cd7aabcb5ce8', '52ef5e79-2eea-4481-9e9c-40e1adde8f3a', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52'),
('56f7e60a-d320-4434-9779-356bb6d06b2b', '52ef5e79-2eea-4481-9e9c-40e1adde8f3a', '53a66484-018a-49f7-9e89-98e8ae3fa0c5'),
('eac31bc1-6b7b-41a2-aae7-c4288248e09e', '878d5747-5a24-451c-9376-f5f9b54852b8', '81f5596d-f30c-4a5d-8341-7f1b5df64c57');

-- 
-- Вывод данных для таблицы group_users
--
INSERT INTO group_users VALUES
('db3d6c6e-49f1-4e14-8e6f-bb5c8f6e3214', '52ef5e79-2eea-4481-9e9c-40e1adde8f3a', '4a386632-3128-45ba-9435-1be9072fe577');

-- 
-- Вывод данных для таблицы identity_resource_claims
--
INSERT INTO identity_resource_claims VALUES
('5846262a-2077-48fd-ba27-86bd9c9d1cf2', '020ba743-92d7-4f65-9bfd-f3fde582749c', '12'),
('a61d1b03-dddb-4279-9d5a-b9e590703fae', '020ba743-92d7-4f65-9bfd-f3fde582749c', '18'),
('e384470b-392e-4916-b275-41710b65f04d', '020ba743-92d7-4f65-9bfd-f3fde582749c', '2'),
('f4c5c90a-82c6-4c11-a7de-53c0beef8c05', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb', '1'),
('ea260c13-9755-4dbd-8872-447a25815d39', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb', '18'),
('ec8df388-cb5e-4472-ae08-b562489c7970', '4ad498c7-14fd-4e3b-9ee4-6019fdcb40fb', '2'),
('16e6862f-f1c6-46e1-9a30-080190244d11', '69790828-313c-4b0c-8f94-9ff3c518d77c', '3'),
('d5aca4cb-494c-46ed-9a95-9f8a90681b1f', '76aea288-aa7c-4f89-8177-65a6d957339c', '1'),
('6d153450-2432-4564-850d-3ff106a0bffa', '76aea288-aa7c-4f89-8177-65a6d957339c', '10'),
('0ea47373-9c7b-4707-bd15-59ff0d41d96f', '76aea288-aa7c-4f89-8177-65a6d957339c', '11'),
('18d30a1e-4260-435b-a4f7-8b1f1dc13d2b', '76aea288-aa7c-4f89-8177-65a6d957339c', '12'),
('7c2fb40f-8fe2-441a-9c7e-70acd5c144a5', '76aea288-aa7c-4f89-8177-65a6d957339c', '13'),
('7b4ad35b-2971-4b92-b486-a4252517005b', '76aea288-aa7c-4f89-8177-65a6d957339c', '14'),
('d989af01-9f14-41df-915a-d6aa310f01bc', '76aea288-aa7c-4f89-8177-65a6d957339c', '15'),
('22299349-4e56-470c-bd8a-fc9948285cba', '76aea288-aa7c-4f89-8177-65a6d957339c', '16'),
('a912c431-444f-42b7-b4b9-6b5114675d41', '76aea288-aa7c-4f89-8177-65a6d957339c', '4'),
('b87d0930-dcc2-4016-acbf-dac5c2d775fe', '76aea288-aa7c-4f89-8177-65a6d957339c', '5'),
('1fce8c04-73d5-4c44-a037-ed76470f9d92', '76aea288-aa7c-4f89-8177-65a6d957339c', '6'),
('ae5b9388-5371-4a83-8910-9b9edb4e2be6', '76aea288-aa7c-4f89-8177-65a6d957339c', '7'),
('d1d2543e-8cb8-489a-96b0-cf2e3acb01d6', '76aea288-aa7c-4f89-8177-65a6d957339c', '8'),
('8053368d-4e88-436b-a617-0b2dda8fb6e0', '76aea288-aa7c-4f89-8177-65a6d957339c', '9'),
('df2e8ab1-2607-4387-8ebe-ec6281510d03', 'd4bf2bb5-96a5-4436-a2e6-0d25e9f12ff7', '11'),
('2faf72ca-565a-4a1d-b94d-f82df08c94d7', 'd4bf2bb5-96a5-4436-a2e6-0d25e9f12ff7', '21'),
('bcfc1825-ca8c-4c09-9ff0-d466905a1ea4', 'e17554f4-145f-469f-a266-9f59aa65a019', '17'),
('f1eaa096-1f95-49a5-9f91-146c77858439', 'e17554f4-145f-469f-a266-9f59aa65a019', '2');

-- 
-- Вывод данных для таблицы object_client
--
INSERT INTO object_client VALUES
('1d0fa978-bb03-4909-9972-3b516234e044', '96e31bb0-481c-4911-bcdb-187ce6dbf19c', '0524bee3-e5b3-44a9-9563-ded53952f0a0'),
('4397cef2-aed0-42d8-affe-503caed20e14', '3dfa1e98-c4f4-4a1d-a830-0c2090382bbf', '0a42db3d-fb51-44c0-a8bc-2e37ea9a12d5'),
('a9f6838b-f04d-43ba-9924-b3e880af41dc', '8745746f-0262-431a-afdf-80afaebb4a6b', '389ec486-bc91-4415-a4cd-84656ac06ca4'),
('8306092a-56ad-4ef9-9f25-b8d1e03c2241', 'b3ccf41a-0a90-4ff8-b596-0e4155f72aba', '49b83ac7-1c55-4a01-8352-6b03061667a7'),
('d4a54c3c-c035-48c2-b285-75e7f52a5bae', '7b3fe03a-9f88-4576-9884-3cab05c67a7c', 'ead71a21-14b3-40bd-ac5f-e004ea6c67af');

-- 
-- Вывод данных для таблицы object_endpoints
--
INSERT INTO object_endpoints VALUES
('0f6c373c-f393-45d7-a9f9-15e872d83f9a', '/api/1.0/themes', '96e31bb0-481c-4911-bcdb-187ce6dbf19c', 'test'),
('94b4f43a-40e2-43c2-b8a2-74688c1e03e0', '/about', '79731b18-204a-45c7-a5dd-943b200a2a08', ''),
('bb1ec515-852d-4dec-a515-6012b5f9d885', '/credit', '79731b18-204a-45c7-a5dd-943b200a2a08', ''),
('d02f8689-b858-446c-a6c6-1d9a8e41ef8d', '/php_site/page3.php', 'b3ccf41a-0a90-4ff8-b596-0e4155f72aba', ''),
('f8380f53-fcb7-4160-90c9-cb1dcdc41fab', '/index', '79731b18-204a-45c7-a5dd-943b200a2a08', '');

-- 
-- Вывод данных для таблицы profile_attribute_claim
--
INSERT INTO profile_attribute_claim VALUES
('3', '10', '14'),
('a065525a-5302-4037-8e30-0c5fb10a3ae5', '11', '21'),
('f6cd5bee-ca10-4d83-b590-622d3255ae61', '12', '15'),
('2', '15', '11'),
('f3750b7d-120c-4276-aece-818fd37e52c5', '17', '31'),
('4', '2', '2'),
('dfcbb5bf-a1fb-4fed-a573-47ef84a98f7f', '2498e40d-1fcd-4ae9-bee2-47decec4cafc', '1'),
('5', '5', '18'),
('1', '7', '19'),
('c729e33c-577f-41a5-9e8d-656e1711f1d1', '8', '20');

-- 
-- Вывод данных для таблицы role_local_partitions
--
INSERT INTO role_local_partitions VALUES
('30', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '05cfd2b0-1f9a-48e5-8e37-60b8292c8de5'),
('17', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '0a9494e8-9e61-4eb8-8b67-ab3a3530a3f0'),
('95c871f6-8154-4607-8184-89d76c44cb5f', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '0d258970-54a8-46ee-ba02-5a216a6ae550'),
('a26f83cf-0ccd-4c56-9822-7f363cab0f23', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '12b474e6-d464-4090-a694-24d70f942edc'),
('0b132f5c-23c5-4602-ae17-b4a2de7b2b0b', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '13a31fa8-a974-4d94-a6d7-ee6563c00521'),
('15', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '20'),
('16', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '21'),
('b6c2d7b1-65d6-4df9-a720-89a2a9ea46b5', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '22'),
('96bff628-e564-42dc-b3a3-03e64945db9a', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '2728548c-db98-459c-bc21-198d7e4a9c2f'),
('24573c48-e35b-4b98-a89b-cbddb6f9de8e', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '2cfa7acb-c532-4121-98b8-0db93f4c3d11'),
('f9835466-93d1-432d-bf01-fc029f14b96b', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '329e4c2c-bc47-4e2d-b9bb-f3cd0bf1114a'),
('34', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '34e09645-37a2-4676-bab0-574bc805beed'),
('065974b0-55ae-4d29-a283-f2f578fafaa6', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '38779ae7-b3e2-479f-88a0-0e7056c3030e'),
('8150d6de-4820-45af-8a48-27aaf26b8017', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '3a0a64b4-de4b-4bc2-9565-4376c26eef9b'),
('3', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '3b9e33c4-6bf4-4c2c-b881-1c6e6bec0f09'),
('ebd4eba0-18cc-4679-b7dc-1194834f72fb', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '3dc881fb-5122-4c5d-8511-973cd1c89937'),
('625e6ba8-f4b2-40ba-ac33-64e9644f2eef', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '41fcda70-7e68-47cb-ba07-99ab1d515580'),
('44', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '455f045a-1bb2-4e59-b285-191213e20eac'),
('c3419530-66a9-4b02-a2fc-78f5df5e8430', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '486c464e-fc7b-4f40-a414-eb1cf086ed2e'),
('c44eee14-905e-4976-b25d-47ae74bcc116', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '4c12e0bd-4bcf-4cc0-ad02-3a9262bfb629'),
('59', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '4ce92ad5-6a32-4372-8f4e-7fbd74017361'),
('c62dce67-4f95-4365-a88f-4f8d79f6db76', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '50f7d720-3244-489b-8926-8584548de102'),
('1fbcc32f-22d3-44e0-916b-44eb17399676', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '515aba47-7542-4cec-b0ed-2333f7711970'),
('58', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '54157f0f-49cc-49e8-9af0-ad8ec99aea88'),
('0958c51d-1db6-43a0-96a8-524157b15798', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '5ba83619-7c96-42aa-a414-1642fc020cd8'),
('e79af1fa-8733-46c1-ac0b-243598913045', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '5ccb1303-3ae6-4b52-813a-0ca06d601b0f'),
('89dd36ac-0b2d-4304-95e3-41f30d2f4c65', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '5da547e9-947e-49c0-a789-63a1bf5f8263'),
('ecd62e31-6b53-46d1-92ae-28e75f1b422c', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '5e973e3d-161b-46a8-8393-1a9849b88ffe'),
('5e61bcdb-af9a-446c-b3b4-fb126feb0a04', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '636d20ec-b95f-4b95-8725-e37321c62e11'),
('a7fb8a3d-9c8c-40d3-a9a2-11667646a620', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '639e01a6-be4c-4409-a208-05e96af2aba8'),
('0576519e-0e8e-49c3-a23b-c61179941dd6', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '6cba36ad-76e7-4b62-b462-768e88529ebb'),
('64c5f7bf-959d-4fa8-a8da-7a67f831e298', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '6e7c8e54-703c-436f-9d01-0c0c5caf6fa5'),
('35', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '74eaddd0-cb0d-4c76-9fa1-9c8ebefb62c1'),
('207b867f-3b21-4395-aa46-e575b93b5535', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '77e1f28a-8a57-476e-a7b6-12b6aa42a215'),
('32', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '78e34768-3aee-4c91-9852-0baba8169cca'),
('413578f8-2cfe-451d-a016-d3c3d6f2ef22', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '79fe3b65-d943-437f-9a43-602fd0981b06'),
('c4401236-af07-411f-85c5-ddb073899b37', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '7f8aa24a-2f92-4328-9a01-bf65b401020b'),
('28', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '80656165-372e-4974-b64a-2a400378fd55'),
('26', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '81157e32-9345-47b6-ba16-895ff6ef693a'),
('4', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8344af6c-1056-4ab0-8edd-046485447a64'),
('fee9e650-36f5-40c5-8341-14a7a5394179', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8cdc2f03-afb2-4bb0-9b8d-77e42c33a1fc'),
('6d32401d-b88e-425a-a70e-ef09c9f57822', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8ddd2f9c-70fb-470a-ace5-a72b7c3d2802'),
('6', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8ea49510-3b70-4589-8758-d9aa2457d08c'),
('29', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8ec97bbd-be26-4a23-9743-bea4504aa77e'),
('11933b41-4514-4ba6-9be7-625f2ec1580a', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '930cfd23-e821-423e-848b-d40af0657399'),
('d20a9ecf-ea53-427c-8c3d-35cc518fb519', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '944ada42-f73a-47d8-9c21-a6f9f7c59486'),
('cb84d225-61c5-42fc-9422-a0b500f7741f', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '9754d1a8-fde8-4b96-987f-669b647e819a'),
('0e480843-bb25-49b3-86d9-0f6914e0ef98', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '995b3a2e-01c1-4de4-a5e8-86e84c4ff7d3'),
('57', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '99f17734-91ac-42d4-8b42-a3e146d1c1cb'),
('ac9e7dce-8e20-49ac-83cd-29ba44acad95', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '9b976d95-0811-4e8b-8f1d-28399f2be301'),
('2', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '9d4bae77-0d4b-4636-8b4d-9db4f8833cea'),
('947c6fa5-25f9-4641-b959-b5910d33e1a1', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '9f0c14e5-3308-4a44-80e2-81497741fbd2'),
('063a7c05-8164-4efa-b790-c8ac91c6cecd', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'a32e89fb-1fda-4d9d-b7b5-4a7c236925bd'),
('3205e22d-2e9e-4272-bf7a-7445196f4fd3', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'a526eb7d-27d2-4e4a-9a84-814024836e67'),
('bd982057-d446-4dc1-b155-c91872f3c597', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'a6d2c951-8cfa-4a5b-acb0-9633b498dbab'),
('23c6fc54-e4be-4a25-baf0-3a1a72f59782', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'aae0a7df-840e-4c10-a7d5-a62a21260296'),
('9c6d46e1-9e57-489c-b893-9d0dd3298045', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'ac85a077-aff7-4bc0-ad2d-7d90b75cdd6f'),
('334cb75a-76d7-47a4-8e8b-13a126967e86', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'b3406789-45d3-4bb4-8d43-fa49e5ab5f0c'),
('8', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'bae054ba-081d-4899-83dd-de8d402d0eda'),
('27', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'c5b45d3b-2b40-4dd8-b4e7-dbd6635d2868'),
('41915380-ba33-49ac-bdb9-cb70f67a9b90', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'c7d28b49-2164-48b1-89ee-4fc828633c0a'),
('ea75e063-e64f-4192-acee-dc64914ca665', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'd29da653-c793-47fc-8355-caa33f08ba2f'),
('de799219-4b02-436d-a2b2-561325338fcc', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'd82e3257-64ca-4c51-96fb-40e3e9fce075'),
('271b66a0-3310-48b8-b5ef-3155c9ffd33b', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'de79937f-b660-46d0-b342-e73247d1e90b'),
('1', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'e569f4e1-c05f-40be-bb5f-5b8dd68ac514'),
('934c6ab3-2636-48f8-ac0a-3242f876e580', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'e77a27ef-f5c4-48fe-8d86-72feb234ca1d'),
('dc6a5a15-4379-423e-8ed6-93be6f54e884', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'ec067a08-d268-47b3-bb1e-be8135744154'),
('5', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'eee84324-5ada-4f3c-bb47-d1640031bda7'),
('33', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'efe36177-15ea-4095-8c5d-67e47e23139c'),
('31e1c2fc-a1cb-4a6b-b4b4-25030efdc1f3', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'f1b6818d-d306-403c-82f5-cfa5edafe96a'),
('47a87719-2a1b-4ff9-83e7-44fa6a4343f0', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'f732134d-338d-484c-8e58-f61a344068f0'),
('32b368f9-0a30-48c6-ac30-dea19722a254', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'fa82c6da-85e9-43ab-8743-0c729242aff2'),
('7', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'fab53125-f253-4b6c-812f-49de1f271f01'),
('f08b9aba-27fb-437b-b697-8d01dec7bb17', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'ff1549ec-1e7c-4375-bc2e-d20c939c8674'),
('ad62ca5d-9b27-47a6-ac6a-88d90bccf87e', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'ff18f2f1-53bc-47bd-8016-72edc40156bb'),
('c2487c42-d38e-4a4e-ae09-58474f31b9e2', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '12b474e6-d464-4090-a694-24d70f942edc'),
('faf0a00f-7595-4dab-925c-54a7f459a8db', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '2728548c-db98-459c-bc21-198d7e4a9c2f'),
('7cb96b0b-eb61-44cd-8553-fdb521edda82', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '455f045a-1bb2-4e59-b285-191213e20eac'),
('990fc413-94c6-45f5-b531-f4447a0ecf7d', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '79fe3b65-d943-437f-9a43-602fd0981b06'),
('812ea592-f177-4ec7-8408-d1c24b4cafbf', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '8cdc2f03-afb2-4bb0-9b8d-77e42c33a1fc'),
('65cd8784-4f77-406a-80f4-3fd0406788a9', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '944ada42-f73a-47d8-9c21-a6f9f7c59486'),
('20ea1651-bb87-462f-abb8-d5e3549cb3d7', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '9f0c14e5-3308-4a44-80e2-81497741fbd2'),
('549edc33-3182-45fb-9053-bd8fdcf989da', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'ac85a077-aff7-4bc0-ad2d-7d90b75cdd6f'),
('32e4a565-3250-419d-92f1-d12707290859', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'e77a27ef-f5c4-48fe-8d86-72feb234ca1d'),
('bc12b0f8-7287-4dd3-a76b-7d1f0d577fb3', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'ec067a08-d268-47b3-bb1e-be8135744154'),
('0449c6a8-52ea-4ff7-9490-f952365e4809', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'ff1549ec-1e7c-4375-bc2e-d20c939c8674'),
('eb13b1ef-990a-4d76-9323-329e16f4142f', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'ff18f2f1-53bc-47bd-8016-72edc40156bb');

-- 
-- Вывод данных для таблицы user_api_keys
--

-- Таблица identityserver4_release.user_api_keys не содержит данных

-- 
-- Вывод данных для таблицы user_claims
--
INSERT INTO user_claims VALUES
('04d04cf5-c061-4eb0-bc87-d04e67ca5b9d', '79b69d3a-677e-43c8-a11c-d472c86e0ab8', '5', 'Алексей'),
('f8cfd853-e2b7-4959-a9c4-a0be0db1b674', '79b69d3a-677e-43c8-a11c-d472c86e0ab8', '4', 'К');

-- 
-- Вывод данных для таблицы user_confirm_codes
--
INSERT INTO user_confirm_codes VALUES
('f21861d8-5432-45ad-8beb-09499b9becb2', '4a386632-3128-45ba-9435-1be9072fe577', '389487', '2017-09-05 14:45:18');

-- 
-- Вывод данных для таблицы user_devices
--

-- Таблица identityserver4_release.user_devices не содержит данных

-- 
-- Вывод данных для таблицы user_logins
--
INSERT INTO user_logins VALUES
('cc6550f7-f312-458c-8943-847db21edbc5', '79b69d3a-677e-43c8-a11c-d472c86e0ab8', 'Google', '115408645423588119210', '');

-- 
-- Вывод данных для таблицы user_roles
--
INSERT INTO user_roles VALUES
('4226276c-9550-4c5a-a98d-94558c5b89ba', '4a386632-3128-45ba-9435-1be9072fe577', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52'),
('1', '79b69d3a-677e-43c8-a11c-d472c86e0ab8', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52');

-- 
-- Вывод данных для таблицы object_endpoint_permissions
--
INSERT INTO object_endpoint_permissions VALUES
('1976f0ad-aaea-4254-bcb9-e0deba75498a', '0f6c373c-f393-45d7-a9f9-15e872d83f9a', 'c1fc9ae1-c544-4e43-b932-f538cbc28519'),
('8439f631-490d-4a22-8930-3b99000202ce', '0f6c373c-f393-45d7-a9f9-15e872d83f9a', 'cb0e2d36-07b2-4cc5-baa2-6400022395ec'),
('1c09c035-498b-48e3-8ebc-0d74c9d8a348', '94b4f43a-40e2-43c2-b8a2-74688c1e03e0', '06573305-97db-4fa1-9db4-edf376d85cb1'),
('b4de88ab-5ef3-40fb-923d-dd3da00fbc35', '94b4f43a-40e2-43c2-b8a2-74688c1e03e0', 'c1fc9ae1-c544-4e43-b932-f538cbc28519'),
('a060801f-b4d0-4411-ab5d-94505963a335', '94b4f43a-40e2-43c2-b8a2-74688c1e03e0', 'cb0e2d36-07b2-4cc5-baa2-6400022395ec'),
('f67ae0d1-137a-406f-82e7-51ee0f71c57d', '94b4f43a-40e2-43c2-b8a2-74688c1e03e0', 'f43194ab-45a9-49c7-b0e7-1788f7efb6ed'),
('9b07f586-1b8f-4a36-a0a0-2bb2b776c8e3', 'bb1ec515-852d-4dec-a515-6012b5f9d885', '06573305-97db-4fa1-9db4-edf376d85cb1'),
('a88828b5-9daa-4add-8a8e-4afa0c4498d3', 'bb1ec515-852d-4dec-a515-6012b5f9d885', 'c1fc9ae1-c544-4e43-b932-f538cbc28519'),
('d709e376-ddd9-49c2-823a-16a5af1264d8', 'bb1ec515-852d-4dec-a515-6012b5f9d885', 'cb0e2d36-07b2-4cc5-baa2-6400022395ec'),
('dca25ab8-c60d-4a26-a150-8bd0df4a04db', 'bb1ec515-852d-4dec-a515-6012b5f9d885', 'f43194ab-45a9-49c7-b0e7-1788f7efb6ed'),
('02a7d028-2001-44aa-84e7-5d61001fdb1e', 'd02f8689-b858-446c-a6c6-1d9a8e41ef8d', 'c1fc9ae1-c544-4e43-b932-f538cbc28519'),
('727b8454-b4ba-4c53-8f6a-9828fe6fb984', 'd02f8689-b858-446c-a6c6-1d9a8e41ef8d', 'cb0e2d36-07b2-4cc5-baa2-6400022395ec'),
('03f2616b-9f39-411c-9d14-9d36d1032fad', 'f8380f53-fcb7-4160-90c9-cb1dcdc41fab', '06573305-97db-4fa1-9db4-edf376d85cb1'),
('1b467c55-d6dc-49a6-88e7-2dd3e17b765c', 'f8380f53-fcb7-4160-90c9-cb1dcdc41fab', 'c1fc9ae1-c544-4e43-b932-f538cbc28519'),
('7a1546ff-76fa-46f5-a993-dc9c86c9fae9', 'f8380f53-fcb7-4160-90c9-cb1dcdc41fab', 'cb0e2d36-07b2-4cc5-baa2-6400022395ec'),
('5e1aa83a-371a-44d3-b906-ef4bf45c585d', 'f8380f53-fcb7-4160-90c9-cb1dcdc41fab', 'f43194ab-45a9-49c7-b0e7-1788f7efb6ed');

-- 
-- Вывод данных для таблицы role_permissions
--
INSERT INTO role_permissions VALUES
('ed85d9d5-d526-494c-88c0-c2fa10acb1f2', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '02a7d028-2001-44aa-84e7-5d61001fdb1e'),
('194b861e-94cd-4c04-9405-8ed612eef700', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '1976f0ad-aaea-4254-bcb9-e0deba75498a'),
('762489f3-02ef-4812-bb8f-ae9943c53677', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '1b467c55-d6dc-49a6-88e7-2dd3e17b765c'),
('6f4e1a48-724a-4193-b282-438ac8199e00', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '5e1aa83a-371a-44d3-b906-ef4bf45c585d'),
('9129a4da-7df6-449e-ac8e-4e11a2a54a64', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '727b8454-b4ba-4c53-8f6a-9828fe6fb984'),
('bb83c259-8c01-4f92-8239-bed50652cff1', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '7a1546ff-76fa-46f5-a993-dc9c86c9fae9'),
('2383704d-bb56-4354-8086-471bc6c00ecd', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', '8439f631-490d-4a22-8930-3b99000202ce'),
('9b10b522-e709-4498-ae67-2289a2b7ab67', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'a060801f-b4d0-4411-ab5d-94505963a335'),
('4576a0fc-b339-432f-9d6e-9f9a7f05e269', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'a88828b5-9daa-4add-8a8e-4afa0c4498d3'),
('bd91d3f8-03cc-46df-8c79-b18deba02828', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'b4de88ab-5ef3-40fb-923d-dd3da00fbc35'),
('1df5667e-963c-4564-9044-c402b691b658', '512c887c-4467-4e7f-aa5c-3fbb41cd9b52', 'dca25ab8-c60d-4a26-a150-8bd0df4a04db'),
('b061b2d4-3c3e-4537-8b46-84b3a9dac850', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '03f2616b-9f39-411c-9d14-9d36d1032fad'),
('17103175-e999-4744-be7b-ab59f65ab9ed', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '1b467c55-d6dc-49a6-88e7-2dd3e17b765c'),
('62af67fe-2697-403f-86c2-bf4dcc0b191d', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '5e1aa83a-371a-44d3-b906-ef4bf45c585d'),
('9eba3faf-4e99-4e34-9731-9066acea4621', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '7a1546ff-76fa-46f5-a993-dc9c86c9fae9'),
('4d716e82-1ccd-4674-8e6c-964dbd2ab88a', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', '9b07f586-1b8f-4a36-a0a0-2bb2b776c8e3'),
('2489d3f4-3b27-4b33-a9cd-e8e032869a27', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'a88828b5-9daa-4add-8a8e-4afa0c4498d3'),
('e6bf5911-7ac8-4411-b05d-64e42fb6f4cf', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'd709e376-ddd9-49c2-823a-16a5af1264d8'),
('6cc81175-a738-4ed6-a6bf-d110527c4cf0', '53a66484-018a-49f7-9e89-98e8ae3fa0c5', 'dca25ab8-c60d-4a26-a150-8bd0df4a04db'),
('6254b204-d2e6-4e56-8dbc-2d14e2641523', '81f5596d-f30c-4a5d-8341-7f1b5df64c57', '02a7d028-2001-44aa-84e7-5d61001fdb1e'),
('858c22fe-9117-4917-a0ac-86f19329161f', '81f5596d-f30c-4a5d-8341-7f1b5df64c57', '727b8454-b4ba-4c53-8f6a-9828fe6fb984');

-- 
-- Восстановить предыдущий режим SQL (SQL mode)
-- 
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;

-- 
-- Включение внешних ключей
-- 
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;