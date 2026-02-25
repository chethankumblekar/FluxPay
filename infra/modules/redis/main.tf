resource "azurerm_redis_cache" "redis" {
  name                = "${replace(var.name_prefix, "-", "")}-redis"
  location            = var.location
  resource_group_name = var.resource_group_name

  capacity            = 0
  family              = "C"
  sku_name            = "Basic"

  minimum_tls_version = "1.2"
  non_ssl_port_enabled = false
  public_network_access_enabled = true

  tags = var.tags
}