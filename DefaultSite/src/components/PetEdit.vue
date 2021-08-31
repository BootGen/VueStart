<template>
  <div class="card col-md-12">
    <div class="row">
      <div class="col-12 d-flex justify-content-end">
        <button type="button" class="btn btn-default btn-sm" @click="save">
          <span class="bi bi-save" aria-hidden="true"></span> Save
        </button>
        <button type="button" class="btn btn-default btn-sm" @click="cancel">
          <span class="bi bi-backspace" aria-hidden="true"></span> Cancel
        </button>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <ul class="list-group list-group-flush">
          <li class="list-group-item">
            <div class="row d-flex align-items-center">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                name:
              </div>
              <div class="col-8 d-flex justify-content-start">
                <input
                  v-model="editedPet.name"
                  type="text"
                  class="form-control"
                  :id="`petName${editedPet.name}`"
                />
              </div>
            </div>
          </li>
          <li class="list-group-item">
            <div class="row d-flex align-items-center">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                species:
              </div>
              <div class="col-8 d-flex justify-content-start">
                <input
                  v-model="editedPet.species"
                  type="text"
                  class="form-control"
                  :id="`species${editedPet.name}`"
                />
              </div>
            </div>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: 'PetView',
  props: {
    modelValue: Object
  },
  setup(props) {
    const editedPet = { ...props.modelValue }
    return { editedPet }
  },
  methods: {
    save() {
      this.$emit('update:modelValue', this.editedPet)
      this.$emit('close')
    },
    cancel() {
      this.$emit('close')
    }
  }
});
</script>