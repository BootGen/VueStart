<template>
  <div class="card">
    <ul class="list-group list-group-flush">
      <li class="list-group-item">
        <button type="button" class="btn btn-default btn-sm" @click="$emit('edit')">
          <span class="bi bi-pencil" aria-hidden="true"></span> Edit
        </button>
        <button type="button" class="btn btn-default btn-sm" @click="$emit('delete')">
          <span class="bi bi-trash" aria-hidden="true"></span> Delete
        </button>
      </li>
      <li class="list-group-item">
        <div class="row">
          <div class="col-4 d-flex justify-content-start text-break text-start">
            UserName:
          </div>
          <div class="col-8 d-flex justify-content-start text-break text-start">
            {{ modelValue.userName }}
          </div>
        </div>
      </li>
      <li class="list-group-item">
        <div class="row">
          <div class="col-4 d-flex justify-content-start text-break text-start">
            Email:
          </div>
          <div class="col-8 d-flex justify-content-start text-break text-start">
            {{ modelValue.email }}
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
import { defineComponent } from "vue";
import PetList from "../components/PetList.vue";

export default defineComponent({
  components: { PetList },
  name: "UserView",
  props: {
    modelValue: Object,
  },
  setup(props, context) {
    const onUpdate = function (pets) {
      const newValue = { ...props.modelValue };
      newValue.pets = pets;
      context.emit("update:modelValue", newValue);
    };
    return { onUpdate };
  },
});
</script>
