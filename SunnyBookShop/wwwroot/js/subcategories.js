var fictionCategories = [
    'Romance',
    'Historical Fiction',
    'Sci-Fi',
    'Thriller',
    'Manga',
    'Classic',
    'Fantasy',
];

var nonFictionCategories = [
    'Biographies',
    'Health and Lifestyle',
    'Art, Creative',
    'Religion',
];

var educationCategories = [
    'Languages',
    'Medical',
    'Mathematics',
    'Religion',
    'Sciences',
    'Physical Education',
];

const categorySelect = document.getElementById('categorySelect');
const subCategorySelect = document.getElementById('subCategorySelect');

const subCategories = {
    '0': fictionCategories,
    '1': nonFictionCategories,
    '2': educationCategories
};

let selectCat = categorySelect.value;
subCategorySelect.innerHTML = '';
subCategories[selectCat].forEach(function (subCategory) {
    const opt = document.createElement('option');
    opt.text = subCategory;
    subCategorySelect.add(opt);
});

categorySelect.addEventListener('change', function () {
    const selectedCategory = categorySelect.value;
    subCategorySelect.innerHTML = '';
    subCategories[selectedCategory].forEach(function (subCategory) {
        const option = document.createElement('option');
        option.text = subCategory;
        subCategorySelect.add(option);
    });
});

subCategorySelect.addEventListener("change", () => {
    console.log(subCategorySelect.value);
});