variable "keyvault_name" {
  type = string
  default = "terraform-keyvaultan67w"
}
variable "state_rg" {
    type = string
    default = "tfstate"
}
variable "storage_secret_name" {
  type = string
  default = "tf-state-storage-key"
}
variable "tfstate_storage_account_name"{
    type = string
    default = "tfstatean67w"
}
variable "container_name"{
    type = string
    default = "tfstate"
}
