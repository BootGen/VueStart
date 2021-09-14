<template>
  <div class="card">
    <ul class="list-group list-group-flush">
      <li class="list-group-item">
        <button type="button" class="btn btn-default btn-sm" @click="save">
          <span class="bi bi-save" aria-hidden="true"></span> Save
        </button>
        <button type="button" class="btn btn-default btn-sm" @click="cancel">
          <span class="bi bi-backspace" aria-hidden="true"></span> Cancel
        </button>
      </li>
      <li class="list-group-item">
        <div class="row d-flex align-items-center">
          <div class="col-4 d-flex justify-content-start text-break text-start">
            UserName:
          </div>
          <div class="col-8 d-flex justify-content-start">
            <input v-model="editedItem.userName" type="text" class="form-control" />
          </div>
        </div>
      </li>
      <li class="list-group-item">
        <div class="row d-flex align-items-center">
          <div class="col-4 d-flex justify-content-start text-break text-start">
            Email:
          </div>
          <div class="col-8 d-flex justify-content-start">
            <input v-model="editedItem.email" type="text" class="form-control" />
          </div>
        </div>
      </li>
      <li class="list-group-item">
        <pet-list
          :modelValue="modelValue.pets"
          @update:modelValue="onUpdate"
        ></pet-list>
      </li>
    </ul>
  </div>
</template>

<script>
import { defineComponent, ref } from "vue";
import PetList from "../components/PetList.vue";

export default defineComponent({
  components: { PetList },
  name: "UserView",
  props: {
    modelValue: Object,
  },
  setup(props, context) {
    const editedItem = ref({ ...props.modelValue });

    const save = function () {
      context.emit("update:modelValue", editedItem.value);
      context.emit("saved");
    };

    const cancel = function () {
      editedItem.value = { ...props.modelValue };
      context.emit("canceled");
    };

    const onUpdate = function (pets) {
      const newValue = { ...props.modelValue };
      newValue.pets = pets;
      context.emit("update:modelValue", newValue);
    };

    return { editedItem, save, cancel, onUpdate };
  },
});
</script>