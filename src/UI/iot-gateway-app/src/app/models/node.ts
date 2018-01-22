import { Config } from '../../config'

export class Node {
    Id?: String
    Label?: String
    Product?: String
    Manufacturer?: String
    GenericType?: String

    constructor(resource) {
        Object.assign(this, resource)
    }

    get DeviceIcon(): String {
        return "assets/" + this.GenericType + ".png";
    }
}
