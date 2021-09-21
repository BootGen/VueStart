const  app = Vue.createApp({
    data() {
      return {
        garfield: {
          name: 'Garfield',
          species: 'Cat'
        }
      }
    }
  });

  app.component('pet-view', {
    template: `
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
        Name:
      </div>
      <div class="col-8 d-flex justify-content-start text-break">
        {{ modelValue.name }}
      </div>
    </div>
  </li>
  <li class="list-group-item">
    <div class="row">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        Species:
      </div>
      <div class="col-8 d-flex justify-content-start text-break">
        {{ modelValue.species }}
      </div>
    </div>
  </li>`,
    name: 'PetView',
    props: {
      modelValue: Object,
    },
    emits: ['edit',  'delete']
  });
  
  app.mount('#app');