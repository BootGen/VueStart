<template>
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
</template>

<script>
import { defineComponent, reactive } from 'vue';

export default defineComponent({
  name: 'UserEdit',
  props: {
    modelValue: Object,
  },
  emits: ['update:modelValue', 'canceled', 'saved'],
  setup(props, context) {
    const editedItem = reactive({ ...props.modelValue });

    function save() {
      context.emit('update:modelValue', editedItem);
      context.emit('saved');
    }
    function cancel() {
      context.emit('canceled');
    }

    return { editedItem, save, cancel };
  },
});
</script>