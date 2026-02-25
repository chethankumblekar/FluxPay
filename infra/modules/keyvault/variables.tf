variable "resource_group_name" {}
variable "location" {}
variable "name_prefix" {}
variable "tenant_id" {}
variable "aks_principal_id" {}
variable "tags" {}
variable "redis_key" {
  sensitive = true
}
variable "sql_password" {
  sensitive = true
}