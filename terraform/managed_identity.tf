resource "azurerm_user_assigned_identity" "hro-repairs-api-vault-access-identity" {
  resource_group_name = var.resource_group_name
  location            = var.resource_group_location
  name       = "hro-repairs-api-vault-access-identity"
}
