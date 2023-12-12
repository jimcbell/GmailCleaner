terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
  backend "azurerm" {
      resource_group_name  = "tfstate"
      storage_account_name = "tfstatean67w"
      container_name       = "tfstate"
      key                  = "terraform.tfstate"
  }

}

provider "azurerm" {
  features {}
}

resource "random_string" "resource_code" {
  length  = 5
  special = false
  upper   = false
}
data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "gmail-cleaner-rg" {
  name     = "gmail-cleaner-rg"
  location = "eastus"
}
resource "azurerm_key_vault" "gmail-cleaner-kv" {
  name                        = "gmail-cleaner-keyvault"
  location                    = azurerm_resource_group.gmail-cleaner-rg.location
  resource_group_name         = azurerm_resource_group.gmail-cleaner-rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = "2129a809-69e9-426b-821d-763137f7b8ee"
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get",
      "List",
      "Update",
      "Create",
      "Import",
      "Delete",
      "Recover",
      "Backup",
      "Restore"
    ]

    secret_permissions = [
      "Get",
      "List",
      "Set",
      "Delete",
      "Recover",
      "Backup",
      "Restore",
      "Purge"
    ]

    storage_permissions = [
      "Get",
    ]
    certificate_permissions = [
      "Get",
      "List",
      "Update",
      "Create",
      "Import",
      "Delete",
      "Recover",
      "Backup",
      "Restore",
      "ManageContacts",
      "ManageIssuers",
      "GetIssuers",
      "ListIssuers",
      "SetIssuers",
      "DeleteIssuers"
    ]
  }
}

resource "azurerm_key_vault_secret" "gmail_cleaner_con_string" {
  name         = "gmail-cleaner-db-connection-string"
  value        = var.gmail_cleaner_con_string
  key_vault_id = azurerm_key_vault.gmail-cleaner-kv.id
}
resource "azurerm_key_vault_secret" "gmail_cleaner_client_id" {
  name         = "gmail-cleaner-client-id"
  value        = var.gmail_cleaner_client_id
  key_vault_id = azurerm_key_vault.gmail-cleaner-kv.id
}
resource "azurerm_key_vault_secret" "gmail_cleaner_client_secret" {
  name         = "gmail-cleaner-client-secret"
  value        = var.gmail_cleaner_client_secret
  key_vault_id = azurerm_key_vault.gmail-cleaner-kv.id
}






# data "azurerm_key_vault" "keyvault" {
#   name                = "${var.keyvault_name}"
#   resource_group_name = "${var.state_rg}"
# }

# data "azurerm_key_vault_secret" "storage_account_access_key" {
#   name         = "${var.storage_secret_name}"
#   key_vault_id = data.azurerm_key_vault.keyvault.id
# }
