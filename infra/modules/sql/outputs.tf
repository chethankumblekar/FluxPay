output "sql_server_name" {
  value = azurerm_mssql_server.sql.name
}

output "sql_admin_password" {
  value     = random_password.sql_admin.result
  sensitive = true
}