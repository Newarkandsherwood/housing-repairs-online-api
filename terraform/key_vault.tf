resource "azurerm_key_vault" "hro-repairs-api-key-vault" {
  name                       = "hro-repairs-api-key-vault"
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
