function CreateBlog() {
    // Ensure Summernote is initialized
    var $contentEditor = $('#blog_content');
    if (!$contentEditor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
        $contentEditor.summernote({
            height: 300,
            minHeight: null,
            maxHeight: null,
            focus: false
        });
    }

    // Validate required fields
    var title = $('#blog_title').val();
    var content = '';
    
    if (typeof $.fn.summernote !== 'undefined' && $contentEditor.hasClass('note-editable')) {
        content = $contentEditor.summernote('code');
    } else {
        content = $contentEditor.val() || '';
    }

    if (!title || title.trim() === '') {
        errorAlert('Blog Title is required');
        return;
    }

    if (!content || content.trim() === '' || content === '<p><br></p>') {
        errorAlert('Blog Content is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Title', title);
    formData.append('Slug', $('#blog_slug').val() || '');
    formData.append('Category', $('#blog_category').val() || '');
    formData.append('Author', $('#blog_author').val() || '');
    
    // Handle published date
    var publishedDate = $('#blog_published_date').val();
    if (publishedDate) {
        formData.append('PublishedDate', new Date(publishedDate).toISOString());
    }
    
    formData.append('Content', content);
    formData.append('Excerpt', $('#blog_excerpt').val() || '');
    formData.append('IsPublished', $('#blog_is_published').is(':checked'));

    // Add cover image if selected
    var coverImageInput = $('#blog_cover_image')[0];
    if (coverImageInput && coverImageInput.files.length > 0) {
        formData.append('CoverImage', coverImageInput.files[0]);
    }

    // Show processing state
    processingBtn('blog_add_blog_btn');

    $.ajax({
        type: 'POST',
        url: '/Blog/CreateBlog',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('blog_add_blog_btn');
            if (result.success) {
                // Reset form
                $('#blog_addModal').modal('hide');
                $('#blog_title').val('');
                $('#blog_slug').val('');
                $('#blog_category').val('');
                $('#blog_author').val('');
                $('#blog_published_date').val('');
                $('#blog_excerpt').val('');
                $('#blog_content').summernote('code', '');
                $('#blog_cover_image').val('');
                $('#blog_is_published').prop('checked', false);
                
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('blog_add_blog_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function GetBlogDetailForEdit(id) {
    if (!id || id === '') {
        errorAlert('Invalid blog ID');
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Blog/GetBlogById',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                var data = result.data;
                $('#blog_edit_Id').val(data.id);
                $('#blog_title_edit').val(data.title);
                $('#blog_slug_edit').val(data.slug || '');
                $('#blog_category_edit').val(data.category || '');
                $('#blog_author_edit').val(data.author || '');
                
                // Handle published date
                if (data.publishedDate) {
                    var publishedDate = new Date(data.publishedDate);
                    var formattedDate = publishedDate.toISOString().slice(0, 16);
                    $('#blog_published_date_edit').val(formattedDate);
                } else {
                    $('#blog_published_date_edit').val('');
                }
                
                $('#blog_excerpt_edit').val(data.excerpt || '');
                $('#blog_content_edit').summernote('code', data.content || '');
                $('#blog_is_published_edit').prop('checked', data.isPublished);

                // Display current cover image
                if (data.coverImageUrl) {
                    $('#blog_current_cover').html(`
                        <p class="text-muted mb-2">Current Cover Image:</p>
                        <img src="/${data.coverImageUrl}" alt="Blog Cover" style="max-width: 300px; max-height: 200px; object-fit: cover; border: 1px solid #ddd; padding: 5px; border-radius: 4px;">
                    `);
                } else {
                    $('#blog_current_cover').html('');
                }

                $('#blog_editModal').modal('show');
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function UpdateBlog() {
    // Ensure Summernote is initialized
    var $contentEditor = $('#blog_content_edit');
    if (!$contentEditor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
        $contentEditor.summernote({
            height: 300,
            minHeight: null,
            maxHeight: null,
            focus: false
        });
    }

    // Validate required fields
    var id = $('#blog_edit_Id').val();
    var title = $('#blog_title_edit').val();
    var content = '';
    
    if (typeof $.fn.summernote !== 'undefined' && $contentEditor.hasClass('note-editable')) {
        content = $contentEditor.summernote('code');
    } else {
        content = $contentEditor.val() || '';
    }

    if (!id || id.trim() === '') {
        errorAlert('Invalid blog ID');
        return;
    }

    if (!title || title.trim() === '') {
        errorAlert('Blog Title is required');
        return;
    }

    if (!content || content.trim() === '' || content === '<p><br></p>') {
        errorAlert('Blog Content is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Id', id);
    formData.append('Title', title);
    formData.append('Slug', $('#blog_slug_edit').val() || '');
    formData.append('Category', $('#blog_category_edit').val() || '');
    formData.append('Author', $('#blog_author_edit').val() || '');
    
    // Handle published date
    var publishedDate = $('#blog_published_date_edit').val();
    if (publishedDate) {
        formData.append('PublishedDate', new Date(publishedDate).toISOString());
    }
    
    formData.append('Content', content);
    formData.append('Excerpt', $('#blog_excerpt_edit').val() || '');
    formData.append('IsPublished', $('#blog_is_published_edit').is(':checked'));

    // Add cover image if new one is selected
    var coverImageInput = $('#blog_cover_image_edit')[0];
    if (coverImageInput && coverImageInput.files.length > 0) {
        formData.append('CoverImage', coverImageInput.files[0]);
    }

    // Show processing state
    processingBtn('blog_edit_blog_btn');

    $.ajax({
        type: 'POST',
        url: '/Blog/UpdateBlog',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('blog_edit_blog_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('blog_edit_blog_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function getDataForDelete(id) {
    $('#blog_delete_Id').val(id);
    $('#blog_deleteModal').modal('show');
}

function DeleteBlog() {
    var id = $('#blog_delete_Id').val();

    if (!id || id === '') {
        errorAlert('Invalid blog ID');
        return;
    }

    processingBtn('blog_del_blog_btn');

    $.ajax({
        type: 'POST',
        url: '/Blog/DeleteBlog',
        data: { id: id },
        success: function (result) {
            returnDefaultBtn('blog_del_blog_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('blog_del_blog_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function ToggleBlogStatus(id) {
    if (!id || id === '') {
        errorAlert('Invalid blog ID');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Blog/ToggleBlogStatus',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

// Initialize Summernote when modal is shown
$(document).ready(function() {
    // Wait for Summernote to be available
    function initializeSummernote() {
        if (typeof $.fn.summernote === 'undefined') {
            // Retry after a short delay if Summernote is not loaded yet
            setTimeout(initializeSummernote, 100);
            return;
        }

        // Initialize Summernote for add modal
        $('#blog_addModal').on('shown.bs.modal', function () {
            var $editor = $('#blog_content');
            if (!$editor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
                $editor.summernote({
                    height: 300,
                    minHeight: null,
                    maxHeight: null,
                    focus: true
                });
            }
        });

        // Initialize Summernote for edit modal
        $('#blog_editModal').on('shown.bs.modal', function () {
            var $editor = $('#blog_content_edit');
            if (!$editor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
                $editor.summernote({
                    height: 300,
                    minHeight: null,
                    maxHeight: null,
                    focus: true
                });
            }
        });

        // Destroy Summernote when modal is hidden to prevent conflicts
        $('#blog_addModal').on('hidden.bs.modal', function () {
            var $editor = $('#blog_content');
            if ($editor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
                $editor.summernote('destroy');
            }
        });

        $('#blog_editModal').on('hidden.bs.modal', function () {
            var $editor = $('#blog_content_edit');
            if ($editor.hasClass('note-editable') && typeof $.fn.summernote !== 'undefined') {
                $editor.summernote('destroy');
            }
        });
    }

    // Start initialization
    initializeSummernote();
});

