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
    COSMOS_CONTAINER_ID                   = azurerm_cosmosdb_sql_container.hro-api-staging.name
    COSMOS_AUTHORIZATION_KEY              = azurerm_cosmosdb_account.hro-api.primary_key
    COSMOS_DATABASE_ID                    = azurerm_cosmosdb_sql_database.hro-api-staging.name
    COSMOS_ENDPOINT_URL                   = azurerm_cosmosdb_account.hro-api.endpoint
    ADDRESSES_API_URL                     = var.addresses_api_url_staging
    ASPNETCORE_ENVIRONMENT                = "staging"
    AUTHENTICATION_IDENTIFIER             = var.authentication_identifier_staging
    AZURE_STORAGE_CONNECTION_STRING       = data.azurerm_storage_account.hro-api.primary_connection_string
    CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = var.confirmation_email_notify_template_id
    CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = var.confirmation_sms_notify_template_id
    DAYS_UNTIL_IMAGE_EXPIRY               = var.days_until_image_expiry_staging
    GOV_NOTIFY_KEY                        = var.gov_notify_key_staging
    INTERNAL_EMAIL                        = var.internal_email_staging
    INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = var.internal_email_notify_template_id
    JWT_SECRET                            = var.jwt_secret_staging
    SCHEDULING_API_URL                    = var.scheduling_api_url_staging
    SENTRY_DSN                            = var.sentry_dsn
    SOR_CONFIGURATION                     = var.sor_configuration_staging
    STORAGE_CONTAINER_NAME                = var.storage_container_name_staging
  }
  auth_settings {
    enabled = false
  }
}

