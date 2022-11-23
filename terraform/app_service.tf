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
    COSMOS_CONTAINER_ID                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-container-id-production.id})"
    COSMOS_AUTHORIZATION_KEY                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-authorization-key.id})"
    COSMOS_DATABASE_ID                              = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-database-id-production.id})"
    COSMOS_ENDPOINT_URL                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-endpoint-url.id})"
    ADDRESSES_API_URL                               = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.addresses-api-url-production.id})"
    ASPNETCORE_ENVIRONMENT                          = "production"
    AUTHENTICATION_IDENTIFIER                       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.authentication-identifier-production.id})"
    AZURE_STORAGE_CONNECTION_STRING                 = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.azure-storage-connection-string.id})"
    TENANT_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-confirmation-email-notify-template-id.id})"
    TENANT_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-confirmation-sms-notify-template-id.id})"
    TENANT_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-internal-email-notify-template-id.id})"
    COMMUNAL_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-confirmation-email-notify-template-id.id})"
    COMMUNAL_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-confirmation-sms-notify-template-id.id})"
    COMMUNAL_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-internal-email-notify-template-id.id})"
    LEASEHOLD_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-confirmation-email-notify-template-id.id})"
    LEASEHOLD_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-confirmation-sms-notify-template-id.id})"
    LEASEHOLD_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-internal-email-notify-template-id.id})"
    DAYS_UNTIL_IMAGE_EXPIRY                         = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.days-until-image-expiry-production.id})"
    GOV_NOTIFY_KEY                                  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.gov-notify-key-production.id})"
    INTERNAL_EMAIL                                  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.internal-email-production.id})"
    JWT_SECRET                                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.jwt-secret-production.id})"
    SCHEDULING_API_URL                              = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.scheduling-api-url-production.id})"
    SENTRY_DSN                                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sentry-dsn.id})"
    SOR_CONFIGURATION_TENANT                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-tenant-production.id})"
    SOR_CONFIGURATION_COMMUNAL                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-communal-production.id})"
    SOR_CONFIGURATION_LEASEHOLD                     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-leasehold-production.id})"
    ALLOWED_APPOINTMENT_SLOTS                       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.allowed-appointment-slots-production.id})"
    STORAGE_CONTAINER_NAME                          = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.storage-container-name-production.id})"
    REPAIR_DAYS_MAPPING                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.repair-days-mapping-production.id})"
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
    COSMOS_CONTAINER_ID                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-container-id-staging.id})"
    COSMOS_AUTHORIZATION_KEY                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-authorization-key.id})"
    COSMOS_DATABASE_ID                              = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-database-id-staging.id})"
    COSMOS_ENDPOINT_URL                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.cosmos-endpoint-url.id})"
    ADDRESSES_API_URL                               = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.addresses-api-url-staging.id})"
    ASPNETCORE_ENVIRONMENT                          = "staging"
    AUTHENTICATION_IDENTIFIER                       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.authentication-identifier-staging.id})"
    AZURE_STORAGE_CONNECTION_STRING                 = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.azure-storage-connection-string.id})"
    TENANT_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-confirmation-email-notify-template-id.id})"
    TENANT_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-confirmation-sms-notify-template-id.id})"
    TENANT_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.tenant-internal-email-notify-template-id.id})"
    COMMUNAL_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-confirmation-email-notify-template-id.id})"
    COMMUNAL_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-confirmation-sms-notify-template-id.id})"
    COMMUNAL_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.communal-internal-email-notify-template-id.id})"
    LEASEHOLD_CONFIRMATION_EMAIL_NOTIFY_TEMPLATE_ID = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-confirmation-email-notify-template-id.id})"
    LEASEHOLD_CONFIRMATION_SMS_NOTIFY_TEMPLATE_ID   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-confirmation-sms-notify-template-id.id})"
    LEASEHOLD_INTERNAL_EMAIL_NOTIFY_TEMPLATE_ID     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.leasehold-internal-email-notify-template-id.id})"
    DAYS_UNTIL_IMAGE_EXPIRY                         = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.days-until-image-expiry-staging.id})"
    GOV_NOTIFY_KEY                                  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.gov-notify-key-staging.id})"
    INTERNAL_EMAIL                                  = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.internal-email-staging.id})"
    JWT_SECRET                                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.jwt-secret-staging.id})"
    SCHEDULING_API_URL                              = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.scheduling-api-url-staging.id})"
    SENTRY_DSN                                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sentry-dsn.id})"
    SOR_CONFIGURATION_TENANT                        = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-tenant-staging.id})"
    SOR_CONFIGURATION_COMMUNAL                      = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-communal-staging.id})"
    SOR_CONFIGURATION_LEASEHOLD                     = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.sor-configuration-leasehold-staging.id})"
    ALLOWED_APPOINTMENT_SLOTS                       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.allowed-appointment-slots-staging.id})"
    STORAGE_CONTAINER_NAME                          = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.storage-container-name-staging.id})"
    REPAIR_DAYS_MAPPING                             = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.repair-days-mapping-staging.id})"
  }

  auth_settings {
    enabled = false
  }
}
