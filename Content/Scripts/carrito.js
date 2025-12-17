// Content/Scripts/carrito.js
// ================================
// GESTIÓN DEL CARRITO MEJORADA
// ================================

// Variables globales del carrito
let cartItems = [];
let cartTotal = 0;

// Inicializar carrito
function initCart() {
    loadCartFromStorage();
    updateCartUI();
}

// Cargar carrito desde sessionStorage
function loadCartFromStorage() {
    const savedCart = sessionStorage.getItem('restaurantCart');
    if (savedCart) {
        cartItems = JSON.parse(savedCart);
        calculateCartTotal();
    }
}

// Guardar carrito en sessionStorage
function saveCartToStorage() {
    sessionStorage.setItem('restaurantCart', JSON.stringify(cartItems));
}

// Calcular total del carrito
function calculateCartTotal() {
    cartTotal = cartItems.reduce((total, item) => {
        return total + (item.price * item.quantity);
    }, 0);
}

// Actualizar UI del carrito
function updateCartUI() {
    const cartBadge = document.getElementById('cartBadge');
    const cartTotalElement = document.querySelector('.cart-total');

    if (cartBadge) {
        const totalItems = cartItems.reduce((sum, item) => sum + item.quantity, 0);
        cartBadge.textContent = totalItems;
        cartBadge.style.display = totalItems > 0 ? 'flex' : 'none';
    }

    if (cartTotalElement) {
        cartTotalElement.textContent = '$' + cartTotal.toFixed(2);
    }
}

// Agregar producto al carrito desde menú
function addToCart(button, productName, productPrice, productId = null) {
    const card = button.closest('.menu-card');
    const quantity = parseInt(card?.querySelector('.quantity-input')?.value || 1);

    // Buscar si el producto ya está en el carrito
    const existingItemIndex = cartItems.findIndex(item =>
        item.name === productName && item.price === productPrice
    );

    if (existingItemIndex > -1) {
        // Incrementar cantidad si ya existe
        cartItems[existingItemIndex].quantity += quantity;
    } else {
        // Agregar nuevo item
        cartItems.push({
            id: productId || Date.now(),
            name: productName,
            price: productPrice,
            quantity: quantity
        });
    }

    // Actualizar storage y UI
    saveCartToStorage();
    calculateCartTotal();
    updateCartUI();

    // Efecto visual de confirmación
    showAddToCartEffect(button);

    // Resetear cantidad
    if (card) {
        card.querySelector('.quantity-input').value = 1;
    }
}

// Efecto visual al agregar al carrito
function showAddToCartEffect(button) {
    const originalHTML = button.innerHTML;
    const originalBg = button.style.background;

    button.innerHTML = '<i class="bi bi-check"></i> Agregado';
    button.style.background = '#198754';
    button.disabled = true;

    setTimeout(() => {
        button.innerHTML = originalHTML;
        button.style.background = originalBg;
        button.disabled = false;
    }, 1500);
}

// Agregar producto desde Home (sin controles de cantidad)
function addToCartFromHome(NombrePlato, Precio, MenuID = null) {
    cartItems.push({
        id: MenuID || Date.now(),
        name: NombrePlato,
        price: Precio,
        quantity: 1
    });

    saveCartToStorage();
    calculateCartTotal();
    updateCartUI();
    showNotification('Producto agregado al carrito', 'success');
}

// Función para Home - Agrega producto y redirige al menú
function orderNow(NombrePlato, Precio, MenuID = null) {
    // 1. Agregar al carrito
    addToCartFromHome(NombrePlato, Precio, MenuID);

    // 2. Redirigir al menú después de 1 segundo
    setTimeout(() => {
        window.location.href = '/menus';
    }, 1000);
}

// Obtener todos los items del carrito
function getCartItems() {
    return cartItems;
}

// Limpiar carrito
function clearCart() {
    cartItems = [];
    saveCartToStorage();
    calculateCartTotal();
    updateCartUI();
    if (document.getElementById('cartItemsContainer')) {
        loadCartPage();
    }
    showNotification('Carrito vaciado', 'info');
}

// Toggle carrito - redirigir a la página del carrito
// Toggle carrito - redirigir a la página del carrito con validación de login
function toggleCart() {
    const cartItems = getCartItems();

    if (cartItems.length === 0) {
        showNotification('Tu carrito está vacío', 'warning');
        return;
    }

    // Verificar si el usuario está autenticado
    if (!isUserAuthenticated()) {
        showNotification('Debes iniciar sesión para proceder al pago', 'warning');
        // Redirigir al login después de un breve delay
        setTimeout(() => {
            window.location.href = '/Account/Login';
        }, 1500);
        return;
    }

    // Si está autenticado, redirigir al carrito
    window.location.href = '/Home/Cart';
}

// Redirigir a la pantalla de checkout si el carrito tiene productos
function goToCheckout() {
    const cart = getCartItems();

    if (!cart || cart.length === 0) {
        showNotification('Tu carrito está vacío', 'warning');
        return;
    }

    if (!isUserAuthenticated()) {
        showNotification('Debes iniciar sesión para proceder al pago', 'warning');
        setTimeout(() => {
            window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent('/Checkout');
        }, 1500);
        return;
    }

    window.location.href = '/Checkout';
}

// Función para verificar si el usuario está autenticado
function isUserAuthenticated() {
    // Verificar si hay un elemento que indique que el usuario está autenticado
    const loginLink = document.getElementById('loginLink');
    const logoutForm = document.getElementById('logoutForm');

    // Si existe el formulario de logout, el usuario está autenticado
    // Si existe el link de login, el usuario NO está autenticado
    return !!logoutForm || !loginLink;
}
// Mostrar notificación
function showNotification(message, type = 'info') {
    // Crear notificación si no existe
    let notification = document.getElementById('cartNotification');
    if (!notification) {
        notification = document.createElement('div');
        notification.id = 'cartNotification';
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            padding: 15px;
            border-radius: 8px;
            color: white;
            font-weight: 600;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            transform: translateX(100%);
            transition: transform 0.3s ease;
        `;
        document.body.appendChild(notification);
    }

    // Configurar colores según tipo
    const colors = {
        success: '#198754',
        warning: '#ffc107',
        error: '#dc3545',
        info: '#0dcaf0'
    };

    notification.style.background = colors[type] || colors.info;
    notification.innerHTML = `
        <strong>${type === 'success' ? '✓' : type === 'warning' ? '⚠' : 'ℹ'}</strong> ${message}
    `;

    // Mostrar notificación
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);

    // Ocultar después de 3 segundos
    setTimeout(() => {
        notification.style.transform = 'translateX(100%)';
    }, 3000);
}

// Funciones para la página del carrito
function loadCartPage() {
    const cart = getCartItems();
    const container = document.getElementById('cartItemsContainer');
    const emptyCart = document.getElementById('emptyCart');

    if (!container || !emptyCart) return;

    if (cart.length === 0) {
        container.style.display = 'none';
        emptyCart.style.display = 'block';
        updateOrderSummary();
        return;
    }

    container.style.display = 'block';
    emptyCart.style.display = 'none';
    container.innerHTML = '';

    cart.forEach((item, index) => {
        const itemHTML = `
            <div class="cart-item card mb-3">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col-md-2">
                            <div class="cart-item-image bg-light rounded" style="width: 80px; height: 80px; display: flex; align-items: center; justify-content: center;">
                                <i class="bi bi-egg-fried" style="font-size: 2rem; color: #6c757d;"></i>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <h6 class="cart-item-name mb-1">${item.name}</h6>
                            <p class="cart-item-price text-success mb-0">$${item.price.toFixed(2)}</p>
                        </div>
                        <div class="col-md-3">
                            <div class="quantity-controls d-flex align-items-center justify-content-center">
                                <button class="quantity-btn btn btn-outline-secondary btn-sm" onclick="decreaseCartQuantity(${index})">
                                    <i class="bi bi-dash"></i>
                                </button>
                                <input type="number" class="quantity-input form-control form-control-sm mx-2 text-center" value="${item.quantity}" readonly style="width: 60px;">
                                <button class="quantity-btn btn btn-outline-secondary btn-sm" onclick="increaseCartQuantity(${index})">
                                    <i class="bi bi-plus"></i>
                                </button>
                            </div>
                        </div>
                        <div class="col-md-2 text-center">
                            <strong class="text-primary">$${(item.price * item.quantity).toFixed(2)}</strong>
                        </div>
                        <div class="col-md-1 text-end">
                            <button class="btn btn-outline-danger btn-sm" onclick="removeFromCart(${index})" title="Eliminar">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        container.innerHTML += itemHTML;
    });

    updateOrderSummary();
}

function increaseCartQuantity(index) {
    let cart = getCartItems();
    if (cart[index].quantity < 99) {
        cart[index].quantity++;
        saveCartToStorage();
        loadCartPage();
        updateCartUI();
    }
}

function decreaseCartQuantity(index) {
    let cart = getCartItems();
    if (cart[index].quantity > 1) {
        cart[index].quantity--;
        saveCartToStorage();
        loadCartPage();
        updateCartUI();
    }
}

function removeFromCart(index) {
    if (confirm('¿Estás seguro de eliminar este producto?')) {
        let cart = getCartItems();
        cart.splice(index, 1);
        saveCartToStorage();
        loadCartPage();
        updateCartUI();
        showNotification('Producto eliminado del carrito', 'success');
    }
}

function updateOrderSummary() {
    const cart = getCartItems();
    const subtotalElement = document.getElementById('subtotal');
    const shippingElement = document.getElementById('shipping');
    const taxElement = document.getElementById('tax');
    const totalElement = document.getElementById('total');

    if (!subtotalElement || !shippingElement || !taxElement || !totalElement) return;

    let subtotal = 0;
    cart.forEach(item => {
        subtotal += item.price * item.quantity;
    });

    const shipping = subtotal > 0 ? 2.50 : 0;
    const tax = subtotal * 0.12; // IVA 12%
    const total = subtotal + shipping + tax;

    subtotalElement.textContent = '$' + subtotal.toFixed(2);
    shippingElement.textContent = '$' + shipping.toFixed(2);
    taxElement.textContent = '$' + tax.toFixed(2);
    totalElement.textContent = '$' + total.toFixed(2);
}
function addToCartAndRedirect(productName, productPrice, productId = null) {
    // Agregar al carrito
    addToCartFromHome(productName, productPrice, productId);

    // Redirigir a la página del menú después de un breve delay
    setTimeout(() => {
        window.location.href = '/menus/Index';
    }, 800); // 0.8 segundos para que el usuario vea la confirmación
}
// Agregar producto desde Home (sin controles de cantidad)
function addToCartFromHome(productName, productPrice, productId = null) {
    cartItems.push({
        id: productId || Date.now(),
        name: productName,
        price: productPrice,
        quantity: 1
    });

    saveCartToStorage();
    calculateCartTotal();
    updateCartUI();
    showNotification('Producto agregado al carrito', 'success');
}
async function proceedToCheckout() {
    const cart = getCartItems();

    if (cart.length === 0) {
        showNotification('Tu carrito está vacío', 'warning');
        return;
    }

    if (!isUserAuthenticated()) {
        showNotification('Debes iniciar sesión para realizar el pago', 'warning');
        setTimeout(() => {
            window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent('/Home/Cart');
        }, 1500);
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const body = new URLSearchParams();
    body.append('cartJson', JSON.stringify(cart));

    try {
        const response = await fetch('/facturas/Checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                ...(token ? { 'RequestVerificationToken': token } : {})
            },
            credentials: 'same-origin',
            body: body.toString()
        });

        const contentType = response.headers.get('content-type') || '';
        if (!contentType.includes('application/json')) {
            window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent('/Home/Cart');
            return;
        }

        const data = await response.json();

        if (data.success && data.redirectUrl) {
            showNotification('Pedido procesado correctamente', 'success');
            clearCart();
            window.location.href = data.redirectUrl;
        } else {
            showNotification(data.message || 'No se pudo procesar el pago', 'error');
        }
    } catch (error) {
        console.error('Error al procesar el pago:', error);
        showNotification('Error al procesar el pago. Inténtalo nuevamente.', 'error');
    }
}
// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function () {
    initCart();

    // Si estamos en la página del carrito, cargar los items
    if (window.location.pathname.includes('Cart')) {
        loadCartPage();
    }
});