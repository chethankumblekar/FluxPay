output "kubelet_object_id" {
  value = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
}

output "aks_name" {
  value = azurerm_kubernetes_cluster.aks.name
}

output "outbound_ip" {
  value = azurerm_public_ip.aks_outbound.ip_address
}