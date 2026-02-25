resource "random_string" "suffix" {
  length  = 4
  upper   = false
  special = false
}

resource "azurerm_resource_group" "rg" {
  name     = "${local.name_prefix}-rg"
  location = var.location
  tags     = local.tags
}

module "monitor" {
  source              = "../../modules/monitor"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags
}

module "network" {
  source              = "../../modules/network"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  address_space       = var.address_space
  tags                = local.tags
}

module "acr" {
  source              = "../../modules/acr"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  suffix              = random_string.suffix.result
  tags                = local.tags
}

module "aks" {
  source              = "../../modules/aks"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  subnet_id           = module.network.aks_subnet_id
  acr_id              = module.acr.acr_id
  log_analytics_workspace_id = module.monitor.workspace_id
  tags                = local.tags
}

module "sql" {
  source              = "../../modules/sql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  aks_outbound_ip     = module.aks.outbound_ip
  tags                = local.tags
}

module "redis" {
  source              = "../../modules/redis"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags
}

data "azurerm_client_config" "current" {}

module "keyvault" {
  source              = "../../modules/keyvault"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  name_prefix         = local.name_prefix
  tenant_id           = data.azurerm_client_config.current.tenant_id
  aks_principal_id    = module.aks.kubelet_object_id
  tags                = local.tags
  redis_key           = module.redis.redis_primary_key
  sql_password        = module.sql.sql_admin_password
}

