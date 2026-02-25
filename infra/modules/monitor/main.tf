resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.name_prefix}-law"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = var.tags
}

resource "azurerm_monitor_action_group" "ag" {
  name                = "${var.name_prefix}-ag"
  resource_group_name = var.resource_group_name
  short_name          = "fpag"
}

resource "azurerm_monitor_metric_alert" "aks_cpu_alert" {
  name                = "${var.name_prefix}-cpu-alert"
  resource_group_name = var.resource_group_name
  scopes              = [azurerm_log_analytics_workspace.law.id]
  severity            = 3
  frequency           = "PT5M"
  window_size         = "PT5M"

  criteria {
    metric_namespace = "Microsoft.ContainerService/managedClusters"
    metric_name      = "node_cpu_usage_percentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }

  action {
    action_group_id = azurerm_monitor_action_group.ag.id
  }
}