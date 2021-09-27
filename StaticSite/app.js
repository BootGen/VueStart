const ref = Vue.ref;
const reactive = Vue.reactive;
const watchEffect = Vue.watchEffect;
const  app = Vue.createApp({
  template: '<user-list v-model="userList"></user-list>',
  name: 'App',
  setup: function () {
    const userList = ref([]);
    function toDataModel(data) {
      return data.map((val, idx) => {
        const v = { ... val }
        for (const property in v) {
          if(Array.isArray(v[property])){
            v[property] = toDataModel(v[property]);
          }
        }
        return {
          id: idx,
          value: v
        }
      })
    }
    function toSimpleArray(data) {
      let arr =  data.map(val => {
        return { ...val.value }
      })
      arr.forEach(item => {
        for (const property in item) {
          if(Array.isArray(item[property])){
            item[property] = toSimpleArray(item[property]);
          }
        }
      })
      return arr;
    }
    function saveToLocalStorage(newValue) {
        let minimized = JSON.stringify(newValue);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
          localStorage.setItem('json', minimized);
        }
    }
    function loadFromLocalStorage() {
      let json = localStorage.getItem('json');
      if (json) {
        const users = JSON.parse(json).users;
        if (users) {
          userList.value = toDataModel(users);
        }
      }
    }
    loadFromLocalStorage();

    watchEffect(() => {
      saveToLocalStorage({
        users: toSimpleArray(userList.value)
      });
    })

    window.addEventListener('storage', loadFromLocalStorage);
    return { userList  }
  },
});

app.component('user-view', {
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
        UserName:
      </div>
      <div class="col-8 d-flex justify-content-start text-break">
        {{ modelValue.userName }}
      </div>
    </div>
  </li>
  <li class="list-group-item">
    <div class="row">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        Email:
      </div>
      <div class="col-8 d-flex justify-content-start text-break">
        {{ modelValue.email }}
      </div>
    </div>
  </li>
  `,
  name: 'UserView',
  props: {
    modelValue: Object,
  },
  emits: ['edit',  'delete']
});

app.component('user-edit', {
  template: `
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
  `,
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

app.component('user-list', {
  template: `
  <div class="row">
    <div class="col-12 d-flex justify-content-between">
      Users:
      <button type="button" class="btn btn-default btn-sm" @click="addNewItem()">
        <span class="bi bi-plus" aria-hidden="true"></span> Add
      </button>
    </div>
    <div class="col-12 mb-2" v-for="item in modelValue" :key="item.id">
      <div class="card">
        <ul class="list-group list-group-flush">
          <user-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedItemId === item.id"></user-edit>
          <user-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></user-view>
          <li class="list-group-item">
          <pet-list v-model="item.value.pets"></pet-list>
          </li>
        </ul>
      </div>
    </div>
    <div class="col-12 mb-2" v-if="newItem != null">
      <div class="card">
        <ul class="list-group list-group-flush">
          <user-edit v-model="newItem" @canceled="cancelAdd" @saved="saveNewItem()"></user-edit>
        </ul>
      </div>
    </div>
  </div>`,
  name: 'UserList',
  props: {
    modelValue: Object
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const editedItemId = ref(-1);
    const newItem = ref(null);

    function editItem(item) {
      editedItemId.value = item.id;
    }
    function deleteItem(item) {
      if (confirm('Are you sure to delete this user?')) {
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id));
      }
    }
    function finishEditing() {
      editedItemId.value = -1;
    }
    function addNewItem() {
      newItem.value = {
        userName: '',
        email: '',
        pets: []
      };
    }
    function saveNewItem() {
        const items = [...props.modelValue];
        const lastId = items.length == 0 ? 0 : items[items.length-1].id;
        items.push({id: lastId + 1, value: newItem.value});
        context.emit('update:modelValue', items);
        newItem.value = null;
    }
    function cancelAdd() {
        newItem.value = null;
    }

    return { editedItemId, newItem, editItem, deleteItem, finishEditing, addNewItem, saveNewItem, cancelAdd };
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
  </li>
  `,
  name: 'PetView',
  props: {
    modelValue: Object,
  },
  emits: ['edit',  'delete']
});

app.component('pet-edit', {
  template: `
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
      Name:
      </div>
      <div class="col-8 d-flex justify-content-start">
        <input v-model="editedItem.name" type="text" class="form-control" />
      </div>
    </div>
  </li>
  <li class="list-group-item">
    <div class="row d-flex align-items-center">
      <div class="col-4 d-flex justify-content-start text-break text-start">
      Species:
      </div>
      <div class="col-8 d-flex justify-content-start">
        <input v-model="editedItem.species" type="text" class="form-control" />
      </div>
    </div>
  </li>
  `,
  name: 'PetEdit',
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

app.component('pet-list', {
  template: `
  <div class="row">
    <div class="col-12 d-flex justify-content-between">
      Pets:
      <button type="button" class="btn btn-default btn-sm" @click="addNewItem()">
        <span class="bi bi-plus" aria-hidden="true"></span> Add
      </button>
    </div>
    <div class="col-12 mb-2" v-for="item in modelValue" :key="item.id">
      <div class="card">
        <ul class="list-group list-group-flush">
          <pet-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedItemId === item.id"></pet-edit>
          <pet-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></pet-view>
        </ul>
      </div>
    </div>
    <div class="col-12 mb-2" v-if="newItem != null">
      <div class="card">
        <ul class="list-group list-group-flush">
          <pet-edit v-model="newItem" @canceled="cancelAdd" @saved="saveNewItem()"></pet-edit>
        </ul>
      </div>
    </div>
  </div>`,
  name: 'PetList',
  props: {
    modelValue: Object
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const editedItemId = ref(-1);
    const newItem = ref(null);

    function editItem(item) {
      editedItemId.value = item.id;
    }
    function deleteItem(item) {
      if (confirm('Are you sure to delete this pet?')) {
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id));
      }
    }
    function finishEditing() {
      editedItemId.value = -1;
    }
    function addNewItem() {
      newItem.value = {
        name: '',
        species: '',
      };
    }
    function saveNewItem() {
        const items = [...props.modelValue];
        const lastId = items.length == 0 ? 0 : items[items.length-1].id;
        items.push({id: lastId + 1, value: newItem.value});
        context.emit('update:modelValue', items);
        newItem.value = null;
    }
    function cancelAdd() {
        newItem.value = null;
    }

    return { editedItemId, newItem, editItem, deleteItem, finishEditing, addNewItem, saveNewItem, cancelAdd };
  }
});

app.mount('#app');