resource "azurerm_service_plan" "hro-api" {
  name                = "hro-api"
  resource_group_name = var.resource_group_name
  location            = var.resource_group_location
  sku_name            = "P1v2"
  os_type             = "Windows"
}

data "azurerm_storage_account" "hro-api" {
  name                = var.storage_account_name
  resource_group_name = var.resource_group_name
}

resource "azurerm_windows_web_app_slot" "hro-api" {
  name           = "staging"
  app_service_id = azurerm_windows_web_app.hro-api.id

  app_settings = {
    COSMOS_CONTAINER_ID                   = azurerm_cosmosdb_sql_container.hro-api-staging.name
    COSMOS_AUTHORIZATION_KEY              = azurerm_cosmosdb_account.hro-api.primary_key
    COSMOS_DATABASE_ID                    = azurerm_cosmosdb_sql_database.hro-api-staging.name
    COSMOS_ENDPOINT_URL                   = azurerm_cosmosdb_account.hro-api.endpoint
    ADDRESSES_API_URL                     = var.address_api_url
    ASPNETCORE_ENVIRONMENT                = var.environment
    AUTHENTICATION_IDENTIFIER             = var.authentication_identifier
    AZURE_STORAGE_CONNECTION_STRING       = data.azurerm_storage_account.hro-api.primary_connection_string
    CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = var.confirmation_email_notify_template_id
    CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = var.confirmation_sms_notify_template_id
    DAYS_UNTIL_IMAGE_EXPIRY               = var.days_until_image_expiry
    GOV_NOTIFY_KEY                        = var.gov_notify_key
    INTERNAL_EMAIL                        = var.internal_email
    INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = var.internal_email_notify_template_id
    JWT_SECRET                            = var.jwt_secret
    SCHEDULING_API_URL                    = var.scheduling_api_url
    SENTRY_DSN                            = var.sentry_dsn
    SOR_CONFIGURATION                     = var.sor_configuration
    STORAGE_CONTAINER_NAME                = "housing-repairs-online-staging"
  }
  site_config {}
}

resource "azurerm_windows_web_app" "hro-api" {
  name                = "hro-api"
  resource_group_name = var.resource_group_name
  location            = var.resource_group_location
  service_plan_id     = azurerm_service_plan.hro-api.id
  site_config {}
  app_settings = {
    COSMOS_CONTAINER_ID                   = azurerm_cosmosdb_sql_container.hro-api.name
    COSMOS_AUTHORIZATION_KEY              = azurerm_cosmosdb_account.hro-api.primary_key
    COSMOS_DATABASE_ID                    = azurerm_cosmosdb_sql_database.hro-api.name
    COSMOS_ENDPOINT_URL                   = azurerm_cosmosdb_account.hro-api.endpoint
    ADDRESSES_API_URL                     = var.address_api_url
    ASPNETCORE_ENVIRONMENT                = var.environment
    AUTHENTICATION_IDENTIFIER             = var.authentication_identifier
    AZURE_STORAGE_CONNECTION_STRING       = data.azurerm_storage_account.hro-api.primary_connection_string
    CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = var.confirmation_email_notify_template_id
    CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = var.confirmation_sms_notify_template_id
    DAYS_UNTIL_IMAGE_EXPIRY               = var.days_until_image_expiry
    GOV_NOTIFY_KEY                        = var.gov_notify_key
    INTERNAL_EMAIL                        = var.internal_email
    INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = var.internal_email_notify_template_id
    JWT_SECRET                            = var.jwt_secret
    SCHEDULING_API_URL                    = var.scheduling_api_url
    SENTRY_DSN                            = var.sentry_dsn
    SOR_CONFIGURATION                     = var.sor_configuration
    STORAGE_CONTAINER_NAME                = "housing-repairs-online"
  }
}
