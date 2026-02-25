resource "random_password" "sql_admin" {
  length  = 20
  special = true
}

resource "azurerm_mssql_server" "sql" {
  name                         = "${replace(var.name_prefix, "-", "")}-sql"
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = "sqladminuser"
  administrator_login_password = random_password.sql_admin.result
  public_network_access_enabled = true
  tags                         = var.tags
}

resource "azurerm_mssql_database" "db" {
  name      = "${var.name_prefix}-db"
  server_id = azurerm_mssql_server.sql.id
  sku_name  = "Basic"
}

resource "azurerm_mssql_firewall_rule" "allow_aks" {
  name             = "AllowAKS"
  server_id        = azurerm_mssql_server.sql.id
  start_ip_address = var.aks_outbound_ip
  end_ip_address   = var.aks_outbound_ip
}