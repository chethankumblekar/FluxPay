resource "azurerm_key_vault" "kv" {
  name                        = "${replace(var.name_prefix, "-", "")}-kv"
  location                    = var.location
  resource_group_name         = var.resource_group_name
  tenant_id                   = var.tenant_id
  sku_name                    = "standard"
  purge_protection_enabled    = false
  soft_delete_retention_days  = 7
  enable_rbac_authorization  = true
  public_network_access_enabled = false
  tags = var.tags
}

# Store Redis Secret
resource "azurerm_key_vault_secret" "redis_key" {
  name         = "redis-primary-key"
  value        = var.redis_key
  key_vault_id = azurerm_key_vault.kv.id
}

# Store SQL Password
resource "azurerm_key_vault_secret" "sql_password" {
  name         = "sql-admin-password"
  value        = var.sql_password
  key_vault_id = azurerm_key_vault.kv.id
}

# Grant AKS access to secrets
resource "azurerm_role_assignment" "aks_kv_access" {
  scope                = azurerm_key_vault.kv.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.aks_principal_id
}