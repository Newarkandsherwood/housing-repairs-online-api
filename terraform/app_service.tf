data "azurerm_service_plan" "hro-app-service-plan" {
  name                = var.service_plan_name
  resource_group_name = var.resource_group_name
}

data "azurerm_storage_account" "hro-api" {
  name                = var.storage_account_name
  resource_group_name = var.resource_group_name
}

resource "azurerm_windows_web_app" "hro-api" {
  name                = var.service_name
  resource_group_name = var.resource_group_name
  location            = var.resource_group_location
  service_plan_id     = data.azurerm_service_plan.hro-app-service-plan.id
  https_only          = true

  site_config {
    health_check_path = "/health"
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.hro-repairs-api-vault-access-identity.id]
  }

  key_vault_reference_identity_id = azurerm_user_assigned_identity.hro-repairs-api-vault-access-identity.id

  app_settings = {
    COSMOS_CONTAINER_ID                   = azurerm_cosmosdb_sql_container.hro-api.name
    COSMOS_AUTHORIZATION_KEY              = azurerm_cosmosdb_account.hro-api.primary_key
    COSMOS_DATABASE_ID                    = azurerm_cosmosdb_sql_database.hro-api.name
    COSMOS_ENDPOINT_URL                   = azurerm_cosmosdb_account.hro-api.endpoint
    ADDRESSES_API_URL                     = var.addresses_api_url_production
    ASPNETCORE_ENVIRONMENT                = "production"
    AUTHENTICATION_IDENTIFIER             = var.authentication_identifier_production
    AZURE_STORAGE_CONNECTION_STRING       = data.azurerm_storage_account.hro-api.primary_connection_string
    CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = var.confirmation_email_notify_template_id
    CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = var.confirmation_sms_notify_template_id
    DAYS_UNTIL_IMAGE_EXPIRY               = var.days_until_image_expiry_production
    GOV_NOTIFY_KEY                        = var.gov_notify_key_production
    INTERNAL_EMAIL                        = var.internal_email_production
    INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = var.internal_email_notify_template_id
    JWT_SECRET                            = var.jwt_secret_production
    SCHEDULING_API_URL                    = var.scheduling_api_url_production
    SENTRY_DSN                            = var.sentry_dsn
    SOR_CONFIGURATION                     = var.sor_configuration_production
    STORAGE_CONTAINER_NAME                = var.storage_container_name_production

  }

  auth_settings {
    enabled = false
  }
}

resource "azurerm_windows_web_app_slot" "hro-api" {
  name           = "Staging"
  app_service_id = azurerm_windows_web_app.hro-api.id
  https_only     = true

  site_config {
    health_check_path = "/health"
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.hro-repairs-api-vault-access-identity.id]
  }

  key_vault_reference_identity_id = azurerm_user_assigned_identity.hro-repairs-api-vault-access-identity.id

  app_settings = {
    COSMOS_CONTAINER_ID                   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-container-id-staging.id})"
    COSMOS_AUTHORIZATION_KEY              = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-authorization-key.id})"
    COSMOS_DATABASE_ID                    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-database-id-staging.id})"
    COSMOS_ENDPOINT_URL                   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-endpoint-url.id})"
    ADDRESSES_API_URL                     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.addresses-api-url-staging.id})"
    ASPNETCORE_ENVIRONMENT                = "staging"
    AUTHENTICATION_IDENTIFIER             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.authentication-identifier-staging.id})"
    AZURE_STORAGE_CONNECTION_STRING       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.azure-storage-connection-string.id})"
    CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.confirmation-email-notify-template-id.id})"
    CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.confirmation-sms-notify-template-id.id})"
    DAYS_UNTIL_IMAGE_EXPIRY               = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.days-until-image-expiry-staging.id})"
    GOV_NOTIFY_KEY                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.gov-notify-key-staging.id})"
    INTERNAL_EMAIL                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.internal-email-staging.id})"
    INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.internal-email-notify-template-id.id})"
    JWT_SECRET                            = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.jwt-secret-staging.id})"
    SCHEDULING_API_URL                    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.scheduling-api-url-staging.id})"
    SENTRY_DSN                            = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sentry-dsn.id})"
    SOR_CONFIGURATION                     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-staging.id})"
    STORAGE_CONTAINER_NAME                = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.storage-container-name-staging.id})"
  }

  auth_settings {
    enabled = false
  }
}
