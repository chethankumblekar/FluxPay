locals {
  region_short = "ci"

  name_prefix = "${var.org}-${var.workload}-${var.environment}-${local.region_short}"

  tags = {
    environment = var.environment
    workload    = var.workload
    managed_by  = "terraform"
  }
}