resource "random_string" "tf_suffix" {
  length  = 4
  upper   = false
  special = false
}

resource "azurerm_resource_group" "tfstate_rg" {
  name     = "fp-tfstate-rg"
  location = "Central India"
}

resource "azurerm_storage_account" "tfstate" {
  name                     = "fptfstate${random_string.tf_suffix.result}"
  resource_group_name      = azurerm_resource_group.tfstate_rg.name
  location                 = azurerm_resource_group.tfstate_rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "tfstate" {
  name                  = "tfstate"
  storage_account_name  = azurerm_storage_account.tfstate.name
  container_access_type = "private"
}