resource "azurerm_key_vault" "hro-repairs-api-key-vault" {
  name                       = "repairs-api-key-vault"
  location                   = var.resource_group_location
  resource_group_name        = var.resource_group_name
  tenant_id                  = var.azure_ad_tenant_id
  soft_delete_retention_days = 7
  purge_protection_enabled   = false
  sku_name                   = "standard"
}

resource "azurerm_key_vault_access_policy" "hro-repairs-api-key-vault-access-policy" {
  key_vault_id = sensitive(azurerm_key_vault.hro-repairs-api-key-vault.id)
  tenant_id    = var.azure_ad_tenant_id
  object_id    = sensitive(azurerm_user_assigned_identity.hro-repairs-api-vault-access-identity.principal_id)

  secret_permissions = [
    "Get",
  ]
}

#=================================
#====== Shared in both environments
resource "azurerm_key_vault_secret" "cosmos-authorization-key" {
  name         = "cosmos-authorization-key"
  value        = azurerm_cosmosdb_account.hro-api.primary_key
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "cosmos-endpoint-url" {
  name         = "cosmos-endpoint-url"
  value        = azurerm_cosmosdb_account.hro-api.endpoint
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "azure-storage-connection-string" {
  name         = "azure-storage-connection-string"
  value        = data.azurerm_storage_account.hro-api.primary_connection_string
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "confirmation-email-notify-template-id" {
  name         = "confirmation-email-notify-template-id"
  value        = var.confirmation_email_notify_template_id
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "confirmation-sms-notify-template-id" {
  name         = "confirmation-sms-notify-template-id"
  value        = var.confirmation_sms_notify_template_id
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "internal-email-notify-template-id" {
  name         = "internal-email-notify-template-id"
  value        = var.internal_email_notify_template_id
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "sentry-dsn" {
  name         = "sentry-dsn"
  value        = var.sentry_dsn
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

#=================================
#====== Staging secrets
resource "azurerm_key_vault_secret" "cosmos-container-id-staging" {
  name         = "cosmos-container-id-staging"
  value        = azurerm_cosmosdb_sql_container.hro-api-staging.name
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "cosmos-database-id-staging" {
  name         = "cosmos-database-id-staging"
  value        = azurerm_cosmosdb_sql_database.hro-api-staging.name
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "addresses-api-url-staging" {
  name         = "addresses-api-url-staging"
  value        = var.addresses_api_url_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "authentication-identifier-staging" {
  name         = "authentication-identifier-staging"
  value        = var.authentication_identifier_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "days-until-image-expiry-staging" {
  name         = "days-until-image-expiry-staging"
  value        = var.days_until_image_expiry_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "gov-notify-key-staging" {
  name         = "gov-notify-key-staging"
  value        = var.gov_notify_key_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "internal-email-staging" {
  name         = "internal-email-staging"
  value        = var.internal_email_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "jwt-secret-staging" {
  name         = "jwt-secret-staging"
  value        = var.jwt_secret_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "scheduling-api-url-staging" {
  name         = "scheduling-api-url-staging"
  value        = var.scheduling_api_url_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "sor-configuration-staging" {
  name         = "sor-configuration-staging"
  value        = var.sor_configuration_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

resource "azurerm_key_vault_secret" "storage-container-name-staging" {
  name         = "storage-container-name-staging"
  value        = var.storage_container_name_staging
  key_vault_id = azurerm_key_vault.hro-repairs-api-key-vault.id
}

#=================================