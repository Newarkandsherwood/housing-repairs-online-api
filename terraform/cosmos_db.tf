resource "azurerm_cosmosdb_account" "hro-api" {
  name                = "hro-api-${var.environment}"
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"
  consistency_policy {
    consistency_level = "Session"
  }
  geo_location {
    location          = var.resource_group_location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "hro-api" {
  name                = "housing-repairs-online"
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.hro-api.name
}

resource "azurerm_cosmosdb_sql_database" "hro-api-staging" {
  name                = "housing-repairs-online-staging"
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.hro-api.name
}
resource "azurerm_cosmosdb_sql_container" "hro-api" {
  name                  = "repairs-requests"
  resource_group_name   = azurerm_cosmosdb_account.hro-api.resource_group_name
  account_name          = azurerm_cosmosdb_account.hro-api.name
  database_name         = azurerm_cosmosdb_sql_database.hro-api.name
  partition_key_path    = "/RepairID"
  partition_key_version = 1
  throughput            = 400

}
resource "azurerm_cosmosdb_sql_container" "hro-api-staging" {
  name                  = "repairs-requests"
  resource_group_name   = azurerm_cosmosdb_account.hro-api.resource_group_name
  account_name          = azurerm_cosmosdb_account.hro-api.name
  database_name         = azurerm_cosmosdb_sql_database.hro-api-staging.name
  partition_key_path    = "/RepairID"
  partition_key_version = 1
  throughput            = 400
}