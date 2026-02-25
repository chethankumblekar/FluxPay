output "aks_subnet_id" {
  value = azurerm_subnet.aks_subnet.id
}

output "data_subnet_id" {
  value = azurerm_subnet.data_subnet.id
}