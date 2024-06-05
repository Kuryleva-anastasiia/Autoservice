const collectionsSwiper = new Swiper(".collections__slider", {
  navigation: {
    nextEl: ".collections__button_next",
    prevEl: ".collections__button_prev",
  },

  breakpoints: {
    320: {
      spaceBetween: 0,
      slidesPerView: 2,
      slidesPerColumn: 2,
      slidesPerGroup: 3,
      grid: {
        rows: 2,
      },
    },
    576: { slidesPerView: 2, spaceBetween: 26, loop: true },
    1e3: { slidesPerView: 3, spaceBetween: 26, loop: true },
    1200: { slidesPerView: 4, spaceBetween: 26, loop: true },
  },
});

var serviceSwiper = new Swiper('.service-slider__container', { loop: true, navigation: { nextEl: '.service-slider__button_next', prevEl: '.service-slider__button_prev', }, effect: "cube", cubeEffect: { shadow: false }, keyboard: { enabled: true, }, });
//var menuButton = document.querySelector(".menu-btn");
//menuButton.addEventListener("click", function () {
//  document
//    .querySelector(".header-wrap__nav-wrap")
//    .classList.toggle("header-wrap__nav-wrap_mobile"),
//    document.querySelector("body").classList.toggle("menu-opened"),
//    document
//      .querySelector(".menu-btn__line_top")
//      .classList.toggle("menu-btn__line_top_rotate"),
//    document
//      .querySelector(".menu-btn__line_middle")
//      .classList.toggle("menu-btn__line_middle_hidden"),
//    document
//      .querySelector(".menu-btn__line_bottom")
//      .classList.toggle("menu-btn__line_bottom_rotate");
//});
// const collectionsSwiper = new Swiper(".collections__slider", {
//     loop: !0,
//     navigation: {
//       nextEl: ".collections__button_next",
//       prevEl: ".collections__button_prev",
//     },
//     options = {
//         breakpoints: {
//         320: {
//             spaceBetween: 10,
//             slidesPerView: 2,
//             slidesPerColumn: 2,
//             slidesPerGroup: 3,
//         },
//         576: { slidesPerView: 2, spaceBetween: 26 },
//         1e3: { slidesPerView: 3, spaceBetween: 26 },
//         1200: { slidesPerView: 4, spaceBetween: 26 },
//         },
//     },
//   }),

// Params

//unreleasedSwiper = new Swiper(".unreleased__slider", {
//  loop: !0,
//  centeredSlides: !0,
//  navigation: {
//    nextEl: ".unreleased__button_next",
//    prevEl: ".unreleased__button_prev",
//  },
//  breakpoints: {
//    320: { slidesPerView: 1 },
//    600: { slidesPerView: 2, spaceBetween: 10 },
//    768: { slidesPerView: 2, spaceBetween: 10 },
//    992: { slidesPerView: 4, spaceBetween: 26 },
//    1200: { slidesPerView: 5, spaceBetween: 26 },
//  },
//});
//$(".form").each(function () {
//  $(this).validate({
//    rules: {
//      name: { minlength: 2 },
//      email: {
//        pattern: /^([a-z0-9_\.-])+@[a-z0-9-]+\.([a-z]{2,4}\.)?[a-z]{2,4}$/i,
//      },
//      phone: { minlength: 18 },
//    },
//    messages: {
//      name: {
//        required: "Please specify your name",
//        minlength: jQuery.validator.format("At least {0} characters required!"),
//      },
//      phone: {
//        required: "Please specify your telephone",
//        minlength: jQuery.validator.format("Incorrect number"),
//      },
//      email: {
//        required: "We need your email address to contact you",
//        email: "Your email address must be in the format of name@domain.com",
//        pattern: "Format of email should be: name@domain.com",
//      },
//    },
//  });
//  $(this).on("input", ".modal__input_name", function () {
//    this.value = this.value.replace(/[^a-z\s]/gi, "");
//  }),
//    $(this).on("input", ".modal__input_tel", function () {
//      this.value.match(/[^0-9]/g) &&
//        (this.value = this.value.replace(/[^0-9( )+-]/g, ""));
//    }),
//    $(".modal__input_tel").mask("+7 (999) 999-99-99");
//});
//var modalButton = $('button[data-toggle="modal"]'),
//  closeModalButton = $(".modal__close");
//function openModal() {
//  document.querySelector("body").classList.add("modal-opened");
//  var e = $(".modal__overlay"),
//    t = $(".modal__dialog");
//  e.addClass("modal__overlay_visible"),
//    t.addClass("modal__dialog_visible"),
//    $(document).on("keydown", function (e) {
//      27 === e.which && closeModal();
//    });
//}
//function closeModal() {
//  document.querySelector("body").classList.remove("modal-opened");
//  var e = $(".modal__overlay"),
//    t = $(".modal__dialog");
//  e.removeClass("modal__overlay_visible"),
//    t.removeClass("modal__dialog_visible");
//}
//modalButton.on("click", function () {
//  openModal();
//}),
//  closeModalButton.on("click", function () {
//    closeModal();
//  });
