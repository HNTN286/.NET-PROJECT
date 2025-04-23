// Common JavaScript for KhoaHoc views
document.addEventListener('DOMContentLoaded', function() {
    // Add staggered animation to cards
    const cards = document.querySelectorAll('.course-card, .public-course-card, .stat-card');
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add('visible');
        }, 100 * index);
    });
    
    // Format currency in the page - Fixed to prevent duplication
    const formatCurrency = (element) => {
        if (!element) return;
        
        // Kiểm tra xem phần tử đã được định dạng chưa
        if (element.dataset.formatted === 'true') return;
        
        const text = element.textContent;
        if (text && text.includes('VNĐ')) {
            // Trích xuất số từ chuỗi, loại bỏ tất cả dấu chấm, dấu phẩy và ký tự không phải số
            const numericPart = text.replace(/[^\d]/g, '');
            
            if (numericPart) {
                // Định dạng số với dấu phân cách hàng nghìn
                const formatted = new Intl.NumberFormat('vi-VN').format(parseInt(numericPart, 10));
                // Cập nhật nội dung với số đã định dạng
                element.textContent = formatted + ' VNĐ';
                // Đánh dấu đã định dạng để tránh định dạng lại
                element.dataset.formatted = 'true';
            }
        }
    };
    
    // Áp dụng định dạng tiền tệ cho các phần tử hiển thị học phí
    document.querySelectorAll('.info-value, .course-info p').forEach(el => {
        if (el.textContent.includes('VNĐ')) {
            formatCurrency(el);
        }
    });
    
    // Định dạng học phí trong form
    const hocPhiInput = document.getElementById('HocPhi');
    if (hocPhiInput) {
        // Định dạng khi trang tải
        if (hocPhiInput.value) {
            const numericValue = hocPhiInput.value.replace(/\D/g, '');
            hocPhiInput.value = new Intl.NumberFormat('vi-VN').format(numericValue);
        }
        
        // Định dạng khi nhập
        hocPhiInput.addEventListener('input', function(e) {
            // Lấy giá trị hiện tại và loại bỏ tất cả ký tự không phải số
            const value = this.value.replace(/\D/g, '');
            // Định dạng lại với dấu phân cách hàng nghìn
            if (value) {
                this.value = new Intl.NumberFormat('vi-VN').format(value);
            }
        });
        
        // Xử lý trước khi submit form
        const form = hocPhiInput.closest('form');
        if (form) {
            form.addEventListener('submit', function() {
                // Loại bỏ tất cả dấu phân cách trước khi gửi
                hocPhiInput.value = hocPhiInput.value.replace(/\D/g, '');
            });
        }
    }
    
    // Add hover effects to buttons
    const buttons = document.querySelectorAll('a[class^="btn-"], button[class^="btn-"]');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-3px)';
            this.style.boxShadow = '0 5px 15px rgba(19, 48, 32, 0.2)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = '';
            this.style.boxShadow = '';
        });
    });
    
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.height = '0';
            alert.style.margin = '0';
            alert.style.padding = '0';
            alert.style.transition = 'all 0.5s ease';
            
            setTimeout(() => {
                alert.style.display = 'none';
            }, 500);
        }, 5000);
    });
    
    // Close alert manually
    document.querySelectorAll('.close-alert').forEach(button => {
        button.addEventListener('click', function() {
            const alert = this.closest('.alert');
            alert.style.opacity = '0';
            setTimeout(() => {
                alert.style.display = 'none';
            }, 300);
        });
    });
    
    // Tìm kiếm khóa học
    const searchInput = document.getElementById('searchCourse');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchText = this.value.toLowerCase();
            let visibleCount = 0;
            
            document.querySelectorAll('.course-card').forEach(card => {
                const courseName = card.dataset.name || '';
                
                if (courseName.includes(searchText)) {
                    card.style.display = '';
                    visibleCount++;
                } else {
                    card.style.display = 'none';
                }
            });
            
            const noResultsElement = document.getElementById('noCourseFound');
            if (noResultsElement) {
                noResultsElement.style.display = visibleCount === 0 ? 'block' : 'none';
            }
        });
        
        // Reset tìm kiếm
        const resetButton = document.getElementById('resetSearch');
        if (resetButton) {
            resetButton.addEventListener('click', function() {
                searchInput.value = '';
                document.querySelectorAll('.course-card').forEach(card => {
                    card.style.display = '';
                });
                
                const noResultsElement = document.getElementById('noCourseFound');
                if (noResultsElement) {
                    noResultsElement.style.display = 'none';
                }
            });
        }
    }
    
    // Scroll animation for elements
    const scrollElements = document.querySelectorAll('.course-card, .stat-card');
    
    const elementInView = (el, dividend = 1) => {
        const elementTop = el.getBoundingClientRect().top;
        return (
            elementTop <= (window.innerHeight || document.documentElement.clientHeight) / dividend
        );
    };
    
    const displayScrollElement = (element) => {
        element.classList.add('visible');
    };
    
    const hideScrollElement = (element) => {
        element.classList.remove('visible');
    };
    
    const handleScrollAnimation = () => {
        scrollElements.forEach((el) => {
            if (elementInView(el, 1.25)) {
                displayScrollElement(el);
            } else {
                hideScrollElement(el);
            }
        });
    };
    
    window.addEventListener('scroll', () => {
        handleScrollAnimation();
    });
    
    // Initialize on page load
    handleScrollAnimation();
});