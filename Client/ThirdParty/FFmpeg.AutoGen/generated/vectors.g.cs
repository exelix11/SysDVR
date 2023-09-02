using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public static unsafe partial class vectors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVABufferSinkParams* av_abuffersink_params_alloc_delegate();
    public static av_abuffersink_params_alloc_delegate av_abuffersink_params_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_add_index_entry_delegate(AVStream* @st, long @pos, long @timestamp, int @size, int @distance, int @flags);
    public static av_add_index_entry_delegate av_add_index_entry;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_add_q_delegate(AVRational @b, AVRational @c);
    public static av_add_q_delegate av_add_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_add_stable_delegate(AVRational @ts_tb, long @ts, AVRational @inc_tb, long @inc);
    public static av_add_stable_delegate av_add_stable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_append_packet_delegate(AVIOContext* @s, AVPacket* @pkt, int @size);
    public static av_append_packet_delegate av_append_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVAudioFifo* av_audio_fifo_alloc_delegate(AVSampleFormat @sample_fmt, int @channels, int @nb_samples);
    public static av_audio_fifo_alloc_delegate av_audio_fifo_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_drain_delegate(AVAudioFifo* @af, int @nb_samples);
    public static av_audio_fifo_drain_delegate av_audio_fifo_drain;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_audio_fifo_free_delegate(AVAudioFifo* @af);
    public static av_audio_fifo_free_delegate av_audio_fifo_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_peek_delegate(AVAudioFifo* @af, void** @data, int @nb_samples);
    public static av_audio_fifo_peek_delegate av_audio_fifo_peek;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_peek_at_delegate(AVAudioFifo* @af, void** @data, int @nb_samples, int @offset);
    public static av_audio_fifo_peek_at_delegate av_audio_fifo_peek_at;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_read_delegate(AVAudioFifo* @af, void** @data, int @nb_samples);
    public static av_audio_fifo_read_delegate av_audio_fifo_read;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_realloc_delegate(AVAudioFifo* @af, int @nb_samples);
    public static av_audio_fifo_realloc_delegate av_audio_fifo_realloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_audio_fifo_reset_delegate(AVAudioFifo* @af);
    public static av_audio_fifo_reset_delegate av_audio_fifo_reset;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_size_delegate(AVAudioFifo* @af);
    public static av_audio_fifo_size_delegate av_audio_fifo_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_space_delegate(AVAudioFifo* @af);
    public static av_audio_fifo_space_delegate av_audio_fifo_space;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_audio_fifo_write_delegate(AVAudioFifo* @af, void** @data, int @nb_samples);
    public static av_audio_fifo_write_delegate av_audio_fifo_write;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_bprint_channel_layout_delegate(AVBPrint* @bp, int @nb_channels, ulong @channel_layout);
    public static av_bprint_channel_layout_delegate av_bprint_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_alloc_delegate(AVBitStreamFilter* @filter, AVBSFContext** @ctx);
    public static av_bsf_alloc_delegate av_bsf_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_bsf_flush_delegate(AVBSFContext* @ctx);
    public static av_bsf_flush_delegate av_bsf_flush;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_bsf_free_delegate(AVBSFContext** @ctx);
    public static av_bsf_free_delegate av_bsf_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBitStreamFilter* av_bsf_get_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_bsf_get_by_name_delegate av_bsf_get_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* av_bsf_get_class_delegate();
    public static av_bsf_get_class_delegate av_bsf_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_get_null_filter_delegate(AVBSFContext** @bsf);
    public static av_bsf_get_null_filter_delegate av_bsf_get_null_filter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_init_delegate(AVBSFContext* @ctx);
    public static av_bsf_init_delegate av_bsf_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBitStreamFilter* av_bsf_iterate_delegate(void** @opaque);
    public static av_bsf_iterate_delegate av_bsf_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBSFList* av_bsf_list_alloc_delegate();
    public static av_bsf_list_alloc_delegate av_bsf_list_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_list_append_delegate(AVBSFList* @lst, AVBSFContext* @bsf);
    public static av_bsf_list_append_delegate av_bsf_list_append;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_list_append2_delegate(AVBSFList* @lst,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @bsf_name, AVDictionary** @options);
    public static av_bsf_list_append2_delegate av_bsf_list_append2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_list_finalize_delegate(AVBSFList** @lst, AVBSFContext** @bsf);
    public static av_bsf_list_finalize_delegate av_bsf_list_finalize;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_bsf_list_free_delegate(AVBSFList** @lst);
    public static av_bsf_list_free_delegate av_bsf_list_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_list_parse_str_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str, AVBSFContext** @bsf);
    public static av_bsf_list_parse_str_delegate av_bsf_list_parse_str;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_receive_packet_delegate(AVBSFContext* @ctx, AVPacket* @pkt);
    public static av_bsf_receive_packet_delegate av_bsf_receive_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_bsf_send_packet_delegate(AVBSFContext* @ctx, AVPacket* @pkt);
    public static av_bsf_send_packet_delegate av_bsf_send_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffer_alloc_delegate(ulong @size);
    public static av_buffer_alloc_delegate av_buffer_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffer_allocz_delegate(ulong @size);
    public static av_buffer_allocz_delegate av_buffer_allocz;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffer_create_delegate(byte* @data, ulong @size, av_buffer_create_free_func @free, void* @opaque, int @flags);
    public static av_buffer_create_delegate av_buffer_create;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_buffer_default_free_delegate(void* @opaque, byte* @data);
    public static av_buffer_default_free_delegate av_buffer_default_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_buffer_get_opaque_delegate(AVBufferRef* @buf);
    public static av_buffer_get_opaque_delegate av_buffer_get_opaque;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffer_get_ref_count_delegate(AVBufferRef* @buf);
    public static av_buffer_get_ref_count_delegate av_buffer_get_ref_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffer_is_writable_delegate(AVBufferRef* @buf);
    public static av_buffer_is_writable_delegate av_buffer_is_writable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffer_make_writable_delegate(AVBufferRef** @buf);
    public static av_buffer_make_writable_delegate av_buffer_make_writable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_buffer_pool_buffer_get_opaque_delegate(AVBufferRef* @ref);
    public static av_buffer_pool_buffer_get_opaque_delegate av_buffer_pool_buffer_get_opaque;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffer_pool_get_delegate(AVBufferPool* @pool);
    public static av_buffer_pool_get_delegate av_buffer_pool_get;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferPool* av_buffer_pool_init_delegate(ulong @size, av_buffer_pool_init_alloc_func @alloc);
    public static av_buffer_pool_init_delegate av_buffer_pool_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferPool* av_buffer_pool_init2_delegate(ulong @size, void* @opaque, av_buffer_pool_init2_alloc_func @alloc, av_buffer_pool_init2_pool_free_func @pool_free);
    public static av_buffer_pool_init2_delegate av_buffer_pool_init2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_buffer_pool_uninit_delegate(AVBufferPool** @pool);
    public static av_buffer_pool_uninit_delegate av_buffer_pool_uninit;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffer_realloc_delegate(AVBufferRef** @buf, ulong @size);
    public static av_buffer_realloc_delegate av_buffer_realloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffer_ref_delegate(AVBufferRef* @buf);
    public static av_buffer_ref_delegate av_buffer_ref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffer_replace_delegate(AVBufferRef** @dst, AVBufferRef* @src);
    public static av_buffer_replace_delegate av_buffer_replace;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_buffer_unref_delegate(AVBufferRef** @buf);
    public static av_buffer_unref_delegate av_buffer_unref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_ch_layout_delegate(AVFilterContext* @ctx, AVChannelLayout* @ch_layout);
    public static av_buffersink_get_ch_layout_delegate av_buffersink_get_ch_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong av_buffersink_get_channel_layout_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_channel_layout_delegate av_buffersink_get_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_channels_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_channels_delegate av_buffersink_get_channels;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_format_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_format_delegate av_buffersink_get_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_frame_delegate(AVFilterContext* @ctx, AVFrame* @frame);
    public static av_buffersink_get_frame_delegate av_buffersink_get_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_frame_flags_delegate(AVFilterContext* @ctx, AVFrame* @frame, int @flags);
    public static av_buffersink_get_frame_flags_delegate av_buffersink_get_frame_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_buffersink_get_frame_rate_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_frame_rate_delegate av_buffersink_get_frame_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_h_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_h_delegate av_buffersink_get_h;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_buffersink_get_hw_frames_ctx_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_hw_frames_ctx_delegate av_buffersink_get_hw_frames_ctx;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_buffersink_get_sample_aspect_ratio_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_sample_aspect_ratio_delegate av_buffersink_get_sample_aspect_ratio;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_sample_rate_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_sample_rate_delegate av_buffersink_get_sample_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_samples_delegate(AVFilterContext* @ctx, AVFrame* @frame, int @nb_samples);
    public static av_buffersink_get_samples_delegate av_buffersink_get_samples;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_buffersink_get_time_base_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_time_base_delegate av_buffersink_get_time_base;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVMediaType av_buffersink_get_type_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_type_delegate av_buffersink_get_type;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersink_get_w_delegate(AVFilterContext* @ctx);
    public static av_buffersink_get_w_delegate av_buffersink_get_w;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferSinkParams* av_buffersink_params_alloc_delegate();
    public static av_buffersink_params_alloc_delegate av_buffersink_params_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_buffersink_set_frame_size_delegate(AVFilterContext* @ctx, uint @frame_size);
    public static av_buffersink_set_frame_size_delegate av_buffersink_set_frame_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersrc_add_frame_delegate(AVFilterContext* @ctx, AVFrame* @frame);
    public static av_buffersrc_add_frame_delegate av_buffersrc_add_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersrc_add_frame_flags_delegate(AVFilterContext* @buffer_src, AVFrame* @frame, int @flags);
    public static av_buffersrc_add_frame_flags_delegate av_buffersrc_add_frame_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersrc_close_delegate(AVFilterContext* @ctx, long @pts, uint @flags);
    public static av_buffersrc_close_delegate av_buffersrc_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_buffersrc_get_nb_failed_requests_delegate(AVFilterContext* @buffer_src);
    public static av_buffersrc_get_nb_failed_requests_delegate av_buffersrc_get_nb_failed_requests;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferSrcParameters* av_buffersrc_parameters_alloc_delegate();
    public static av_buffersrc_parameters_alloc_delegate av_buffersrc_parameters_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersrc_parameters_set_delegate(AVFilterContext* @ctx, AVBufferSrcParameters* @param);
    public static av_buffersrc_parameters_set_delegate av_buffersrc_parameters_set;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_buffersrc_write_frame_delegate(AVFilterContext* @ctx, AVFrame* @frame);
    public static av_buffersrc_write_frame_delegate av_buffersrc_write_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_calloc_delegate(ulong @nmemb, ulong @size);
    public static av_calloc_delegate av_calloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_description_delegate(byte* @buf, ulong @buf_size, AVChannel @channel);
    public static av_channel_description_delegate av_channel_description;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_channel_description_bprint_delegate(AVBPrint* @bp, AVChannel @channel_id);
    public static av_channel_description_bprint_delegate av_channel_description_bprint;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVChannel av_channel_from_string_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_channel_from_string_delegate av_channel_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVChannel av_channel_layout_channel_from_index_delegate(AVChannelLayout* @channel_layout, uint @idx);
    public static av_channel_layout_channel_from_index_delegate av_channel_layout_channel_from_index;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVChannel av_channel_layout_channel_from_string_delegate(AVChannelLayout* @channel_layout,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_channel_layout_channel_from_string_delegate av_channel_layout_channel_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_check_delegate(AVChannelLayout* @channel_layout);
    public static av_channel_layout_check_delegate av_channel_layout_check;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_compare_delegate(AVChannelLayout* @chl, AVChannelLayout* @chl1);
    public static av_channel_layout_compare_delegate av_channel_layout_compare;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_copy_delegate(AVChannelLayout* @dst, AVChannelLayout* @src);
    public static av_channel_layout_copy_delegate av_channel_layout_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_channel_layout_default_delegate(AVChannelLayout* @ch_layout, int @nb_channels);
    public static av_channel_layout_default_delegate av_channel_layout_default;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_describe_delegate(AVChannelLayout* @channel_layout, byte* @buf, ulong @buf_size);
    public static av_channel_layout_describe_delegate av_channel_layout_describe;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_describe_bprint_delegate(AVChannelLayout* @channel_layout, AVBPrint* @bp);
    public static av_channel_layout_describe_bprint_delegate av_channel_layout_describe_bprint;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong av_channel_layout_extract_channel_delegate(ulong @channel_layout, int @index);
    public static av_channel_layout_extract_channel_delegate av_channel_layout_extract_channel;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_from_mask_delegate(AVChannelLayout* @channel_layout, ulong @mask);
    public static av_channel_layout_from_mask_delegate av_channel_layout_from_mask;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_from_string_delegate(AVChannelLayout* @channel_layout,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str);
    public static av_channel_layout_from_string_delegate av_channel_layout_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_index_from_channel_delegate(AVChannelLayout* @channel_layout, AVChannel @channel);
    public static av_channel_layout_index_from_channel_delegate av_channel_layout_index_from_channel;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_layout_index_from_string_delegate(AVChannelLayout* @channel_layout,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_channel_layout_index_from_string_delegate av_channel_layout_index_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVChannelLayout* av_channel_layout_standard_delegate(void** @opaque);
    public static av_channel_layout_standard_delegate av_channel_layout_standard;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong av_channel_layout_subset_delegate(AVChannelLayout* @channel_layout, ulong @mask);
    public static av_channel_layout_subset_delegate av_channel_layout_subset;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_channel_layout_uninit_delegate(AVChannelLayout* @channel_layout);
    public static av_channel_layout_uninit_delegate av_channel_layout_uninit;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_channel_name_delegate(byte* @buf, ulong @buf_size, AVChannel @channel);
    public static av_channel_name_delegate av_channel_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_channel_name_bprint_delegate(AVBPrint* @bp, AVChannel @channel_id);
    public static av_channel_name_bprint_delegate av_channel_name_bprint;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_chroma_location_from_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_chroma_location_from_name_delegate av_chroma_location_from_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_chroma_location_name_delegate(AVChromaLocation @location);
    public static av_chroma_location_name_delegate av_chroma_location_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecID av_codec_get_id_delegate(AVCodecTag** @tags, uint @tag);
    public static av_codec_get_id_delegate av_codec_get_id;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_codec_get_tag_delegate(AVCodecTag** @tags, AVCodecID @id);
    public static av_codec_get_tag_delegate av_codec_get_tag;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_codec_get_tag2_delegate(AVCodecTag** @tags, AVCodecID @id, uint* @tag);
    public static av_codec_get_tag2_delegate av_codec_get_tag2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_codec_is_decoder_delegate(AVCodec* @codec);
    public static av_codec_is_decoder_delegate av_codec_is_decoder;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_codec_is_encoder_delegate(AVCodec* @codec);
    public static av_codec_is_encoder_delegate av_codec_is_encoder;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodec* av_codec_iterate_delegate(void** @opaque);
    public static av_codec_iterate_delegate av_codec_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_color_primaries_from_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_color_primaries_from_name_delegate av_color_primaries_from_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_color_primaries_name_delegate(AVColorPrimaries @primaries);
    public static av_color_primaries_name_delegate av_color_primaries_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_color_range_from_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_color_range_from_name_delegate av_color_range_from_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_color_range_name_delegate(AVColorRange @range);
    public static av_color_range_name_delegate av_color_range_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_color_space_from_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_color_space_from_name_delegate av_color_space_from_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_color_space_name_delegate(AVColorSpace @space);
    public static av_color_space_name_delegate av_color_space_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_color_transfer_from_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_color_transfer_from_name_delegate av_color_transfer_from_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_color_transfer_name_delegate(AVColorTransferCharacteristic @transfer);
    public static av_color_transfer_name_delegate av_color_transfer_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_compare_mod_delegate(ulong @a, ulong @b, ulong @mod);
    public static av_compare_mod_delegate av_compare_mod;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_compare_ts_delegate(long @ts_a, AVRational @tb_a, long @ts_b, AVRational @tb_b);
    public static av_compare_ts_delegate av_compare_ts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVContentLightMetadata* av_content_light_metadata_alloc_delegate(ulong* @size);
    public static av_content_light_metadata_alloc_delegate av_content_light_metadata_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVContentLightMetadata* av_content_light_metadata_create_side_data_delegate(AVFrame* @frame);
    public static av_content_light_metadata_create_side_data_delegate av_content_light_metadata_create_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCPBProperties* av_cpb_properties_alloc_delegate(ulong* @size);
    public static av_cpb_properties_alloc_delegate av_cpb_properties_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_cpu_count_delegate();
    public static av_cpu_count_delegate av_cpu_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_cpu_force_count_delegate(int @count);
    public static av_cpu_force_count_delegate av_cpu_force_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong av_cpu_max_align_delegate();
    public static av_cpu_max_align_delegate av_cpu_max_align;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_d2q_delegate(double @d, int @max);
    public static av_d2q_delegate av_d2q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVD3D11VAContext* av_d3d11va_alloc_context_delegate();
    public static av_d3d11va_alloc_context_delegate av_d3d11va_alloc_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClassCategory av_default_get_category_delegate(void* @ptr);
    public static av_default_get_category_delegate av_default_get_category;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_default_item_name_delegate(void* @ctx);
    public static av_default_item_name_delegate av_default_item_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_demuxer_iterate_delegate(void** @opaque);
    public static av_demuxer_iterate_delegate av_demuxer_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_copy_delegate(AVDictionary** @dst, AVDictionary* @src, int @flags);
    public static av_dict_copy_delegate av_dict_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_count_delegate(AVDictionary* @m);
    public static av_dict_count_delegate av_dict_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_dict_free_delegate(AVDictionary** @m);
    public static av_dict_free_delegate av_dict_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVDictionaryEntry* av_dict_get_delegate(AVDictionary* @m,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key, AVDictionaryEntry* @prev, int @flags);
    public static av_dict_get_delegate av_dict_get;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_get_string_delegate(AVDictionary* @m, byte** @buffer, byte @key_val_sep, byte @pairs_sep);
    public static av_dict_get_string_delegate av_dict_get_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_parse_string_delegate(AVDictionary** @pm,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key_val_sep,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @pairs_sep, int @flags);
    public static av_dict_parse_string_delegate av_dict_parse_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_set_delegate(AVDictionary** @pm,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @value, int @flags);
    public static av_dict_set_delegate av_dict_set;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dict_set_int_delegate(AVDictionary** @pm,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key, long @value, int @flags);
    public static av_dict_set_int_delegate av_dict_set_int;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_disposition_from_string_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @disp);
    public static av_disposition_from_string_delegate av_disposition_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_disposition_to_string_delegate(int @disposition);
    public static av_disposition_to_string_delegate av_disposition_to_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_div_q_delegate(AVRational @b, AVRational @c);
    public static av_div_q_delegate av_div_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_dump_format_delegate(AVFormatContext* @ic, int @index,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, int @is_output);
    public static av_dump_format_delegate av_dump_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVDynamicHDRPlus* av_dynamic_hdr_plus_alloc_delegate(ulong* @size);
    public static av_dynamic_hdr_plus_alloc_delegate av_dynamic_hdr_plus_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVDynamicHDRPlus* av_dynamic_hdr_plus_create_side_data_delegate(AVFrame* @frame);
    public static av_dynamic_hdr_plus_create_side_data_delegate av_dynamic_hdr_plus_create_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_dynarray_add_delegate(void* @tab_ptr, int* @nb_ptr, void* @elem);
    public static av_dynarray_add_delegate av_dynarray_add;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_dynarray_add_nofree_delegate(void* @tab_ptr, int* @nb_ptr, void* @elem);
    public static av_dynarray_add_nofree_delegate av_dynarray_add_nofree;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_dynarray2_add_delegate(void** @tab_ptr, int* @nb_ptr, ulong @elem_size, byte* @elem_data);
    public static av_dynarray2_add_delegate av_dynarray2_add;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_fast_malloc_delegate(void* @ptr, uint* @size, ulong @min_size);
    public static av_fast_malloc_delegate av_fast_malloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_fast_mallocz_delegate(void* @ptr, uint* @size, ulong @min_size);
    public static av_fast_mallocz_delegate av_fast_mallocz;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_fast_padded_malloc_delegate(void* @ptr, uint* @size, ulong @min_size);
    public static av_fast_padded_malloc_delegate av_fast_padded_malloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_fast_padded_mallocz_delegate(void* @ptr, uint* @size, ulong @min_size);
    public static av_fast_padded_mallocz_delegate av_fast_padded_mallocz;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_fast_realloc_delegate(void* @ptr, uint* @size, ulong @min_size);
    public static av_fast_realloc_delegate av_fast_realloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_file_map_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename, byte** @bufptr, ulong* @size, int @log_offset, void* @log_ctx);
    public static av_file_map_delegate av_file_map;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_file_unmap_delegate(byte* @bufptr, ulong @size);
    public static av_file_unmap_delegate av_file_unmap;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_filename_number_test_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename);
    public static av_filename_number_test_delegate av_filename_number_test;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilter* av_filter_iterate_delegate(void** @opaque);
    public static av_filter_iterate_delegate av_filter_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat av_find_best_pix_fmt_of_2_delegate(AVPixelFormat @dst_pix_fmt1, AVPixelFormat @dst_pix_fmt2, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr);
    public static av_find_best_pix_fmt_of_2_delegate av_find_best_pix_fmt_of_2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_find_best_stream_delegate(AVFormatContext* @ic, AVMediaType @type, int @wanted_stream_nb, int @related_stream, AVCodec** @decoder_ret, int @flags);
    public static av_find_best_stream_delegate av_find_best_stream;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_find_default_stream_index_delegate(AVFormatContext* @s);
    public static av_find_default_stream_index_delegate av_find_default_stream_index;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_find_input_format_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @short_name);
    public static av_find_input_format_delegate av_find_input_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_find_nearest_q_idx_delegate(AVRational @q, AVRational* @q_list);
    public static av_find_nearest_q_idx_delegate av_find_nearest_q_idx;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVProgram* av_find_program_from_stream_delegate(AVFormatContext* @ic, AVProgram* @last, int @s);
    public static av_find_program_from_stream_delegate av_find_program_from_stream;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVDurationEstimationMethod av_fmt_ctx_get_duration_estimation_method_delegate(AVFormatContext* @ctx);
    public static av_fmt_ctx_get_duration_estimation_method_delegate av_fmt_ctx_get_duration_estimation_method;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate _iobuf* av_fopen_utf8_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @path,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @mode);
    public static av_fopen_utf8_delegate av_fopen_utf8;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_force_cpu_flags_delegate(int @flags);
    public static av_force_cpu_flags_delegate av_force_cpu_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_format_inject_global_side_data_delegate(AVFormatContext* @s);
    public static av_format_inject_global_side_data_delegate av_format_inject_global_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_fourcc_make_string_delegate(byte* @buf, uint @fourcc);
    public static av_fourcc_make_string_delegate av_fourcc_make_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFrame* av_frame_alloc_delegate();
    public static av_frame_alloc_delegate av_frame_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_apply_cropping_delegate(AVFrame* @frame, int @flags);
    public static av_frame_apply_cropping_delegate av_frame_apply_cropping;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFrame* av_frame_clone_delegate(AVFrame* @src);
    public static av_frame_clone_delegate av_frame_clone;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_copy_delegate(AVFrame* @dst, AVFrame* @src);
    public static av_frame_copy_delegate av_frame_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_copy_props_delegate(AVFrame* @dst, AVFrame* @src);
    public static av_frame_copy_props_delegate av_frame_copy_props;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_frame_free_delegate(AVFrame** @frame);
    public static av_frame_free_delegate av_frame_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_get_buffer_delegate(AVFrame* @frame, int @align);
    public static av_frame_get_buffer_delegate av_frame_get_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_frame_get_plane_buffer_delegate(AVFrame* @frame, int @plane);
    public static av_frame_get_plane_buffer_delegate av_frame_get_plane_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFrameSideData* av_frame_get_side_data_delegate(AVFrame* @frame, AVFrameSideDataType @type);
    public static av_frame_get_side_data_delegate av_frame_get_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_is_writable_delegate(AVFrame* @frame);
    public static av_frame_is_writable_delegate av_frame_is_writable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_make_writable_delegate(AVFrame* @frame);
    public static av_frame_make_writable_delegate av_frame_make_writable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_frame_move_ref_delegate(AVFrame* @dst, AVFrame* @src);
    public static av_frame_move_ref_delegate av_frame_move_ref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFrameSideData* av_frame_new_side_data_delegate(AVFrame* @frame, AVFrameSideDataType @type, ulong @size);
    public static av_frame_new_side_data_delegate av_frame_new_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFrameSideData* av_frame_new_side_data_from_buf_delegate(AVFrame* @frame, AVFrameSideDataType @type, AVBufferRef* @buf);
    public static av_frame_new_side_data_from_buf_delegate av_frame_new_side_data_from_buf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_frame_ref_delegate(AVFrame* @dst, AVFrame* @src);
    public static av_frame_ref_delegate av_frame_ref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_frame_remove_side_data_delegate(AVFrame* @frame, AVFrameSideDataType @type);
    public static av_frame_remove_side_data_delegate av_frame_remove_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_frame_side_data_name_delegate(AVFrameSideDataType @type);
    public static av_frame_side_data_name_delegate av_frame_side_data_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_frame_unref_delegate(AVFrame* @frame);
    public static av_frame_unref_delegate av_frame_unref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_free_delegate(void* @ptr);
    public static av_free_delegate av_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_freep_delegate(void* @ptr);
    public static av_freep_delegate av_freep;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_gcd_delegate(long @a, long @b);
    public static av_gcd_delegate av_gcd;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_gcd_q_delegate(AVRational @a, AVRational @b, int @max_den, AVRational @def);
    public static av_gcd_q_delegate av_gcd_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVSampleFormat av_get_alt_sample_fmt_delegate(AVSampleFormat @sample_fmt, int @planar);
    public static av_get_alt_sample_fmt_delegate av_get_alt_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_audio_frame_duration_delegate(AVCodecContext* @avctx, int @frame_bytes);
    public static av_get_audio_frame_duration_delegate av_get_audio_frame_duration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_audio_frame_duration2_delegate(AVCodecParameters* @par, int @frame_bytes);
    public static av_get_audio_frame_duration2_delegate av_get_audio_frame_duration2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_bits_per_pixel_delegate(AVPixFmtDescriptor* @pixdesc);
    public static av_get_bits_per_pixel_delegate av_get_bits_per_pixel;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_bits_per_sample_delegate(AVCodecID @codec_id);
    public static av_get_bits_per_sample_delegate av_get_bits_per_sample;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_bytes_per_sample_delegate(AVSampleFormat @sample_fmt);
    public static av_get_bytes_per_sample_delegate av_get_bytes_per_sample;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_channel_description_delegate(ulong @channel);
    public static av_get_channel_description_delegate av_get_channel_description;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong av_get_channel_layout_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_get_channel_layout_delegate av_get_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_channel_layout_channel_index_delegate(ulong @channel_layout, ulong @channel);
    public static av_get_channel_layout_channel_index_delegate av_get_channel_layout_channel_index;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_channel_layout_nb_channels_delegate(ulong @channel_layout);
    public static av_get_channel_layout_nb_channels_delegate av_get_channel_layout_nb_channels;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_get_channel_layout_string_delegate(byte* @buf, int @buf_size, int @nb_channels, ulong @channel_layout);
    public static av_get_channel_layout_string_delegate av_get_channel_layout_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_channel_name_delegate(ulong @channel);
    public static av_get_channel_name_delegate av_get_channel_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_colorspace_name_delegate(AVColorSpace @val);
    public static av_get_colorspace_name_delegate av_get_colorspace_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_cpu_flags_delegate();
    public static av_get_cpu_flags_delegate av_get_cpu_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_get_default_channel_layout_delegate(int @nb_channels);
    public static av_get_default_channel_layout_delegate av_get_default_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_exact_bits_per_sample_delegate(AVCodecID @codec_id);
    public static av_get_exact_bits_per_sample_delegate av_get_exact_bits_per_sample;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_extended_channel_layout_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, ulong* @channel_layout, int* @nb_channels);
    public static av_get_extended_channel_layout_delegate av_get_extended_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_frame_filename_delegate(byte* @buf, int @buf_size,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @path, int @number);
    public static av_get_frame_filename_delegate av_get_frame_filename;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_frame_filename2_delegate(byte* @buf, int @buf_size,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @path, int @number, int @flags);
    public static av_get_frame_filename2_delegate av_get_frame_filename2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_media_type_string_delegate(AVMediaType @media_type);
    public static av_get_media_type_string_delegate av_get_media_type_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_output_timestamp_delegate(AVFormatContext* @s, int @stream, long* @dts, long* @wall);
    public static av_get_output_timestamp_delegate av_get_output_timestamp;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVSampleFormat av_get_packed_sample_fmt_delegate(AVSampleFormat @sample_fmt);
    public static av_get_packed_sample_fmt_delegate av_get_packed_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_packet_delegate(AVIOContext* @s, AVPacket* @pkt, int @size);
    public static av_get_packet_delegate av_get_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_padded_bits_per_pixel_delegate(AVPixFmtDescriptor* @pixdesc);
    public static av_get_padded_bits_per_pixel_delegate av_get_padded_bits_per_pixel;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecID av_get_pcm_codec_delegate(AVSampleFormat @fmt, int @be);
    public static av_get_pcm_codec_delegate av_get_pcm_codec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte av_get_picture_type_char_delegate(AVPictureType @pict_type);
    public static av_get_picture_type_char_delegate av_get_picture_type_char;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat av_get_pix_fmt_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_get_pix_fmt_delegate av_get_pix_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_pix_fmt_loss_delegate(AVPixelFormat @dst_pix_fmt, AVPixelFormat @src_pix_fmt, int @has_alpha);
    public static av_get_pix_fmt_loss_delegate av_get_pix_fmt_loss;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_pix_fmt_name_delegate(AVPixelFormat @pix_fmt);
    public static av_get_pix_fmt_name_delegate av_get_pix_fmt_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_get_pix_fmt_string_delegate(byte* @buf, int @buf_size, AVPixelFormat @pix_fmt);
    public static av_get_pix_fmt_string_delegate av_get_pix_fmt_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVSampleFormat av_get_planar_sample_fmt_delegate(AVSampleFormat @sample_fmt);
    public static av_get_planar_sample_fmt_delegate av_get_planar_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_profile_name_delegate(AVCodec* @codec, int @profile);
    public static av_get_profile_name_delegate av_get_profile_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVSampleFormat av_get_sample_fmt_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_get_sample_fmt_delegate av_get_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_get_sample_fmt_name_delegate(AVSampleFormat @sample_fmt);
    public static av_get_sample_fmt_name_delegate av_get_sample_fmt_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_get_sample_fmt_string_delegate(byte* @buf, int @buf_size, AVSampleFormat @sample_fmt);
    public static av_get_sample_fmt_string_delegate av_get_sample_fmt_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_get_standard_channel_layout_delegate(uint @index, ulong* @layout, byte** @name);
    public static av_get_standard_channel_layout_delegate av_get_standard_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_get_time_base_q_delegate();
    public static av_get_time_base_q_delegate av_get_time_base_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_gettime_delegate();
    public static av_gettime_delegate av_gettime;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_gettime_relative_delegate();
    public static av_gettime_relative_delegate av_gettime_relative;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_gettime_relative_is_monotonic_delegate();
    public static av_gettime_relative_is_monotonic_delegate av_gettime_relative_is_monotonic;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_grow_packet_delegate(AVPacket* @pkt, int @grow_by);
    public static av_grow_packet_delegate av_grow_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecID av_guess_codec_delegate(AVOutputFormat* @fmt,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @short_name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @mime_type, AVMediaType @type);
    public static av_guess_codec_delegate av_guess_codec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOutputFormat* av_guess_format_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @short_name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @mime_type);
    public static av_guess_format_delegate av_guess_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_guess_frame_rate_delegate(AVFormatContext* @ctx, AVStream* @stream, AVFrame* @frame);
    public static av_guess_frame_rate_delegate av_guess_frame_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_guess_sample_aspect_ratio_delegate(AVFormatContext* @format, AVStream* @stream, AVFrame* @frame);
    public static av_guess_sample_aspect_ratio_delegate av_guess_sample_aspect_ratio;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_hex_dump_delegate(_iobuf* @f, byte* @buf, int @size);
    public static av_hex_dump_delegate av_hex_dump;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_hex_dump_log_delegate(void* @avcl, int @level, byte* @buf, int @size);
    public static av_hex_dump_log_delegate av_hex_dump_log;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_hwdevice_ctx_alloc_delegate(AVHWDeviceType @type);
    public static av_hwdevice_ctx_alloc_delegate av_hwdevice_ctx_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwdevice_ctx_create_delegate(AVBufferRef** @device_ctx, AVHWDeviceType @type,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @device, AVDictionary* @opts, int @flags);
    public static av_hwdevice_ctx_create_delegate av_hwdevice_ctx_create;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwdevice_ctx_create_derived_delegate(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, int @flags);
    public static av_hwdevice_ctx_create_derived_delegate av_hwdevice_ctx_create_derived;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwdevice_ctx_create_derived_opts_delegate(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, AVDictionary* @options, int @flags);
    public static av_hwdevice_ctx_create_derived_opts_delegate av_hwdevice_ctx_create_derived_opts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwdevice_ctx_init_delegate(AVBufferRef* @ref);
    public static av_hwdevice_ctx_init_delegate av_hwdevice_ctx_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVHWDeviceType av_hwdevice_find_type_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_hwdevice_find_type_by_name_delegate av_hwdevice_find_type_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVHWFramesConstraints* av_hwdevice_get_hwframe_constraints_delegate(AVBufferRef* @ref, void* @hwconfig);
    public static av_hwdevice_get_hwframe_constraints_delegate av_hwdevice_get_hwframe_constraints;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_hwdevice_get_type_name_delegate(AVHWDeviceType @type);
    public static av_hwdevice_get_type_name_delegate av_hwdevice_get_type_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_hwdevice_hwconfig_alloc_delegate(AVBufferRef* @device_ctx);
    public static av_hwdevice_hwconfig_alloc_delegate av_hwdevice_hwconfig_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVHWDeviceType av_hwdevice_iterate_types_delegate(AVHWDeviceType @prev);
    public static av_hwdevice_iterate_types_delegate av_hwdevice_iterate_types;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_hwframe_constraints_free_delegate(AVHWFramesConstraints** @constraints);
    public static av_hwframe_constraints_free_delegate av_hwframe_constraints_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVBufferRef* av_hwframe_ctx_alloc_delegate(AVBufferRef* @device_ctx);
    public static av_hwframe_ctx_alloc_delegate av_hwframe_ctx_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_ctx_create_derived_delegate(AVBufferRef** @derived_frame_ctx, AVPixelFormat @format, AVBufferRef* @derived_device_ctx, AVBufferRef* @source_frame_ctx, int @flags);
    public static av_hwframe_ctx_create_derived_delegate av_hwframe_ctx_create_derived;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_ctx_init_delegate(AVBufferRef* @ref);
    public static av_hwframe_ctx_init_delegate av_hwframe_ctx_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_get_buffer_delegate(AVBufferRef* @hwframe_ctx, AVFrame* @frame, int @flags);
    public static av_hwframe_get_buffer_delegate av_hwframe_get_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_map_delegate(AVFrame* @dst, AVFrame* @src, int @flags);
    public static av_hwframe_map_delegate av_hwframe_map;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_transfer_data_delegate(AVFrame* @dst, AVFrame* @src, int @flags);
    public static av_hwframe_transfer_data_delegate av_hwframe_transfer_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_hwframe_transfer_get_formats_delegate(AVBufferRef* @hwframe_ctx, AVHWFrameTransferDirection @dir, AVPixelFormat** @formats, int @flags);
    public static av_hwframe_transfer_get_formats_delegate av_hwframe_transfer_get_formats;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_alloc_delegate(ref byte_ptrArray4 @pointers, ref int_array4 @linesizes, int @w, int @h, AVPixelFormat @pix_fmt, int @align);
    public static av_image_alloc_delegate av_image_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_check_sar_delegate(uint @w, uint @h, AVRational @sar);
    public static av_image_check_sar_delegate av_image_check_sar;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_check_size_delegate(uint @w, uint @h, int @log_offset, void* @log_ctx);
    public static av_image_check_size_delegate av_image_check_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_check_size2_delegate(uint @w, uint @h, long @max_pixels, AVPixelFormat @pix_fmt, int @log_offset, void* @log_ctx);
    public static av_image_check_size2_delegate av_image_check_size2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_image_copy_delegate(ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in int_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height);
    public static av_image_copy_delegate av_image_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_image_copy_plane_delegate(byte* @dst, int @dst_linesize, byte* @src, int @src_linesize, int @bytewidth, int @height);
    public static av_image_copy_plane_delegate av_image_copy_plane;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_image_copy_plane_uc_from_delegate(byte* @dst, long @dst_linesize, byte* @src, long @src_linesize, long @bytewidth, int @height);
    public static av_image_copy_plane_uc_from_delegate av_image_copy_plane_uc_from;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_copy_to_buffer_delegate(byte* @dst, int @dst_size, in byte_ptrArray4 @src_data, in int_array4 @src_linesize, AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    public static av_image_copy_to_buffer_delegate av_image_copy_to_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_image_copy_uc_from_delegate(ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in long_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height);
    public static av_image_copy_uc_from_delegate av_image_copy_uc_from;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_fill_arrays_delegate(ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesize, byte* @src, AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    public static av_image_fill_arrays_delegate av_image_fill_arrays;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_fill_black_delegate(ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesize, AVPixelFormat @pix_fmt, AVColorRange @range, int @width, int @height);
    public static av_image_fill_black_delegate av_image_fill_black;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_fill_linesizes_delegate(ref int_array4 @linesizes, AVPixelFormat @pix_fmt, int @width);
    public static av_image_fill_linesizes_delegate av_image_fill_linesizes;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_image_fill_max_pixsteps_delegate(ref int_array4 @max_pixsteps, ref int_array4 @max_pixstep_comps, AVPixFmtDescriptor* @pixdesc);
    public static av_image_fill_max_pixsteps_delegate av_image_fill_max_pixsteps;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_fill_plane_sizes_delegate(ref ulong_array4 @size, AVPixelFormat @pix_fmt, int @height, in long_array4 @linesizes);
    public static av_image_fill_plane_sizes_delegate av_image_fill_plane_sizes;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_fill_pointers_delegate(ref byte_ptrArray4 @data, AVPixelFormat @pix_fmt, int @height, byte* @ptr, in int_array4 @linesizes);
    public static av_image_fill_pointers_delegate av_image_fill_pointers;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_get_buffer_size_delegate(AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    public static av_image_get_buffer_size_delegate av_image_get_buffer_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_image_get_linesize_delegate(AVPixelFormat @pix_fmt, int @width, int @plane);
    public static av_image_get_linesize_delegate av_image_get_linesize;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_index_search_timestamp_delegate(AVStream* @st, long @timestamp, int @flags);
    public static av_index_search_timestamp_delegate av_index_search_timestamp;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_init_packet_delegate(AVPacket* @pkt);
    public static av_init_packet_delegate av_init_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_input_audio_device_next_delegate(AVInputFormat* @d);
    public static av_input_audio_device_next_delegate av_input_audio_device_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_input_video_device_next_delegate(AVInputFormat* @d);
    public static av_input_video_device_next_delegate av_input_video_device_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_int_list_length_for_size_delegate(uint @elsize, void* @list, ulong @term);
    public static av_int_list_length_for_size_delegate av_int_list_length_for_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_interleaved_write_frame_delegate(AVFormatContext* @s, AVPacket* @pkt);
    public static av_interleaved_write_frame_delegate av_interleaved_write_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_interleaved_write_uncoded_frame_delegate(AVFormatContext* @s, int @stream_index, AVFrame* @frame);
    public static av_interleaved_write_uncoded_frame_delegate av_interleaved_write_uncoded_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_delegate(void* @avcl, int @level,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt);
    public static av_log_delegate av_log;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_default_callback_delegate(void* @avcl, int @level,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt, byte* @vl);
    public static av_log_default_callback_delegate av_log_default_callback;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_format_line_delegate(void* @ptr, int @level,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix);
    public static av_log_format_line_delegate av_log_format_line;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_log_format_line2_delegate(void* @ptr, int @level,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix);
    public static av_log_format_line2_delegate av_log_format_line2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_log_get_flags_delegate();
    public static av_log_get_flags_delegate av_log_get_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_log_get_level_delegate();
    public static av_log_get_level_delegate av_log_get_level;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_once_delegate(void* @avcl, int @initial_level, int @subsequent_level, int* @state,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt);
    public static av_log_once_delegate av_log_once;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_set_callback_delegate(av_log_set_callback_callback_func @callback);
    public static av_log_set_callback_delegate av_log_set_callback;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_set_flags_delegate(int @arg);
    public static av_log_set_flags_delegate av_log_set_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_log_set_level_delegate(int @level);
    public static av_log_set_level_delegate av_log_set_level;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_log2_delegate(uint @v);
    public static av_log2_delegate av_log2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_log2_16bit_delegate(uint @v);
    public static av_log2_16bit_delegate av_log2_16bit;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_malloc_delegate(ulong @size);
    public static av_malloc_delegate av_malloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_malloc_array_delegate(ulong @nmemb, ulong @size);
    public static av_malloc_array_delegate av_malloc_array;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_mallocz_delegate(ulong @size);
    public static av_mallocz_delegate av_mallocz;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_mallocz_array_delegate(ulong @nmemb, ulong @size);
    public static av_mallocz_array_delegate av_mallocz_array;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVMasteringDisplayMetadata* av_mastering_display_metadata_alloc_delegate();
    public static av_mastering_display_metadata_alloc_delegate av_mastering_display_metadata_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVMasteringDisplayMetadata* av_mastering_display_metadata_create_side_data_delegate(AVFrame* @frame);
    public static av_mastering_display_metadata_create_side_data_delegate av_mastering_display_metadata_create_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_match_ext_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @extensions);
    public static av_match_ext_delegate av_match_ext;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_max_alloc_delegate(ulong @max);
    public static av_max_alloc_delegate av_max_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_memcpy_backptr_delegate(byte* @dst, int @back, int @cnt);
    public static av_memcpy_backptr_delegate av_memcpy_backptr;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_memdup_delegate(void* @p, ulong @size);
    public static av_memdup_delegate av_memdup;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_mul_q_delegate(AVRational @b, AVRational @c);
    public static av_mul_q_delegate av_mul_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOutputFormat* av_muxer_iterate_delegate(void** @opaque);
    public static av_muxer_iterate_delegate av_muxer_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_nearer_q_delegate(AVRational @q, AVRational @q1, AVRational @q2);
    public static av_nearer_q_delegate av_nearer_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_new_packet_delegate(AVPacket* @pkt, int @size);
    public static av_new_packet_delegate av_new_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVProgram* av_new_program_delegate(AVFormatContext* @s, int @id);
    public static av_new_program_delegate av_new_program;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* av_opt_child_class_iterate_delegate(AVClass* @parent, void** @iter);
    public static av_opt_child_class_iterate_delegate av_opt_child_class_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_opt_child_next_delegate(void* @obj, void* @prev);
    public static av_opt_child_next_delegate av_opt_child_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_copy_delegate(void* @dest, void* @src);
    public static av_opt_copy_delegate av_opt_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_double_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, double* @double_out);
    public static av_opt_eval_double_delegate av_opt_eval_double;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_flags_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, int* @flags_out);
    public static av_opt_eval_flags_delegate av_opt_eval_flags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_float_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, float* @float_out);
    public static av_opt_eval_float_delegate av_opt_eval_float;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_int_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, int* @int_out);
    public static av_opt_eval_int_delegate av_opt_eval_int;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_int64_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, long* @int64_out);
    public static av_opt_eval_int64_delegate av_opt_eval_int64;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_eval_q_delegate(void* @obj, AVOption* @o,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, AVRational* @q_out);
    public static av_opt_eval_q_delegate av_opt_eval_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOption* av_opt_find_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @unit, int @opt_flags, int @search_flags);
    public static av_opt_find_delegate av_opt_find;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOption* av_opt_find2_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @unit, int @opt_flags, int @search_flags, void** @target_obj);
    public static av_opt_find2_delegate av_opt_find2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_flag_is_set_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @field_name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @flag_name);
    public static av_opt_flag_is_set_delegate av_opt_flag_is_set;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_opt_free_delegate(void* @obj);
    public static av_opt_free_delegate av_opt_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_opt_freep_ranges_delegate(AVOptionRanges** @ranges);
    public static av_opt_freep_ranges_delegate av_opt_freep_ranges;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, byte** @out_val);
    public static av_opt_get_delegate av_opt_get;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_channel_layout_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, long* @ch_layout);
    public static av_opt_get_channel_layout_delegate av_opt_get_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_chlayout_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVChannelLayout* @layout);
    public static av_opt_get_chlayout_delegate av_opt_get_chlayout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_dict_val_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVDictionary** @out_val);
    public static av_opt_get_dict_val_delegate av_opt_get_dict_val;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_double_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, double* @out_val);
    public static av_opt_get_double_delegate av_opt_get_double;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_image_size_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, int* @w_out, int* @h_out);
    public static av_opt_get_image_size_delegate av_opt_get_image_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_int_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, long* @out_val);
    public static av_opt_get_int_delegate av_opt_get_int;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_key_value_delegate(byte** @ropts,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key_val_sep,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @pairs_sep, uint @flags, byte** @rkey, byte** @rval);
    public static av_opt_get_key_value_delegate av_opt_get_key_value;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_pixel_fmt_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVPixelFormat* @out_fmt);
    public static av_opt_get_pixel_fmt_delegate av_opt_get_pixel_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_q_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVRational* @out_val);
    public static av_opt_get_q_delegate av_opt_get_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_sample_fmt_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVSampleFormat* @out_fmt);
    public static av_opt_get_sample_fmt_delegate av_opt_get_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_get_video_rate_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags, AVRational* @out_val);
    public static av_opt_get_video_rate_delegate av_opt_get_video_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_is_set_to_default_delegate(void* @obj, AVOption* @o);
    public static av_opt_is_set_to_default_delegate av_opt_is_set_to_default;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_is_set_to_default_by_name_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @search_flags);
    public static av_opt_is_set_to_default_by_name_delegate av_opt_is_set_to_default_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOption* av_opt_next_delegate(void* @obj, AVOption* @prev);
    public static av_opt_next_delegate av_opt_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_opt_ptr_delegate(AVClass* @avclass, void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static av_opt_ptr_delegate av_opt_ptr;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_query_ranges_delegate(AVOptionRanges** @p0, void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key, int @flags);
    public static av_opt_query_ranges_delegate av_opt_query_ranges;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_query_ranges_default_delegate(AVOptionRanges** @p0, void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key, int @flags);
    public static av_opt_query_ranges_default_delegate av_opt_query_ranges_default;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_serialize_delegate(void* @obj, int @opt_flags, int @flags, byte** @buffer, byte @key_val_sep, byte @pairs_sep);
    public static av_opt_serialize_delegate av_opt_serialize;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @val, int @search_flags);
    public static av_opt_set_delegate av_opt_set;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_bin_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, byte* @val, int @size, int @search_flags);
    public static av_opt_set_bin_delegate av_opt_set_bin;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_channel_layout_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, long @ch_layout, int @search_flags);
    public static av_opt_set_channel_layout_delegate av_opt_set_channel_layout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_chlayout_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVChannelLayout* @layout, int @search_flags);
    public static av_opt_set_chlayout_delegate av_opt_set_chlayout;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_opt_set_defaults_delegate(void* @s);
    public static av_opt_set_defaults_delegate av_opt_set_defaults;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_opt_set_defaults2_delegate(void* @s, int @mask, int @flags);
    public static av_opt_set_defaults2_delegate av_opt_set_defaults2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_dict_delegate(void* @obj, AVDictionary** @options);
    public static av_opt_set_dict_delegate av_opt_set_dict;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_dict_val_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVDictionary* @val, int @search_flags);
    public static av_opt_set_dict_val_delegate av_opt_set_dict_val;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_dict2_delegate(void* @obj, AVDictionary** @options, int @search_flags);
    public static av_opt_set_dict2_delegate av_opt_set_dict2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_double_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, double @val, int @search_flags);
    public static av_opt_set_double_delegate av_opt_set_double;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_from_string_delegate(void* @ctx,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @opts, byte** @shorthand,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key_val_sep,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @pairs_sep);
    public static av_opt_set_from_string_delegate av_opt_set_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_image_size_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @w, int @h, int @search_flags);
    public static av_opt_set_image_size_delegate av_opt_set_image_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_int_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, long @val, int @search_flags);
    public static av_opt_set_int_delegate av_opt_set_int;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_pixel_fmt_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVPixelFormat @fmt, int @search_flags);
    public static av_opt_set_pixel_fmt_delegate av_opt_set_pixel_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_q_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVRational @val, int @search_flags);
    public static av_opt_set_q_delegate av_opt_set_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_sample_fmt_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVSampleFormat @fmt, int @search_flags);
    public static av_opt_set_sample_fmt_delegate av_opt_set_sample_fmt;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_set_video_rate_delegate(void* @obj,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, AVRational @val, int @search_flags);
    public static av_opt_set_video_rate_delegate av_opt_set_video_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_opt_show2_delegate(void* @obj, void* @av_log_obj, int @req_flags, int @rej_flags);
    public static av_opt_show2_delegate av_opt_show2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOutputFormat* av_output_audio_device_next_delegate(AVOutputFormat* @d);
    public static av_output_audio_device_next_delegate av_output_audio_device_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVOutputFormat* av_output_video_device_next_delegate(AVOutputFormat* @d);
    public static av_output_video_device_next_delegate av_output_video_device_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_add_side_data_delegate(AVPacket* @pkt, AVPacketSideDataType @type, byte* @data, ulong @size);
    public static av_packet_add_side_data_delegate av_packet_add_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPacket* av_packet_alloc_delegate();
    public static av_packet_alloc_delegate av_packet_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPacket* av_packet_clone_delegate(AVPacket* @src);
    public static av_packet_clone_delegate av_packet_clone;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_copy_props_delegate(AVPacket* @dst, AVPacket* @src);
    public static av_packet_copy_props_delegate av_packet_copy_props;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_packet_free_delegate(AVPacket** @pkt);
    public static av_packet_free_delegate av_packet_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_packet_free_side_data_delegate(AVPacket* @pkt);
    public static av_packet_free_side_data_delegate av_packet_free_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_from_data_delegate(AVPacket* @pkt, byte* @data, int @size);
    public static av_packet_from_data_delegate av_packet_from_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_packet_get_side_data_delegate(AVPacket* @pkt, AVPacketSideDataType @type, ulong* @size);
    public static av_packet_get_side_data_delegate av_packet_get_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_make_refcounted_delegate(AVPacket* @pkt);
    public static av_packet_make_refcounted_delegate av_packet_make_refcounted;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_make_writable_delegate(AVPacket* @pkt);
    public static av_packet_make_writable_delegate av_packet_make_writable;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_packet_move_ref_delegate(AVPacket* @dst, AVPacket* @src);
    public static av_packet_move_ref_delegate av_packet_move_ref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_packet_new_side_data_delegate(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size);
    public static av_packet_new_side_data_delegate av_packet_new_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_packet_pack_dictionary_delegate(AVDictionary* @dict, ulong* @size);
    public static av_packet_pack_dictionary_delegate av_packet_pack_dictionary;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_ref_delegate(AVPacket* @dst, AVPacket* @src);
    public static av_packet_ref_delegate av_packet_ref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_packet_rescale_ts_delegate(AVPacket* @pkt, AVRational @tb_src, AVRational @tb_dst);
    public static av_packet_rescale_ts_delegate av_packet_rescale_ts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_shrink_side_data_delegate(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size);
    public static av_packet_shrink_side_data_delegate av_packet_shrink_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_packet_side_data_name_delegate(AVPacketSideDataType @type);
    public static av_packet_side_data_name_delegate av_packet_side_data_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_packet_unpack_dictionary_delegate(byte* @data, ulong @size, AVDictionary** @dict);
    public static av_packet_unpack_dictionary_delegate av_packet_unpack_dictionary;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_packet_unref_delegate(AVPacket* @pkt);
    public static av_packet_unref_delegate av_packet_unref;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_parse_cpu_caps_delegate(uint* @flags,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @s);
    public static av_parse_cpu_caps_delegate av_parse_cpu_caps;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_parser_close_delegate(AVCodecParserContext* @s);
    public static av_parser_close_delegate av_parser_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecParserContext* av_parser_init_delegate(int @codec_id);
    public static av_parser_init_delegate av_parser_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecParser* av_parser_iterate_delegate(void** @opaque);
    public static av_parser_iterate_delegate av_parser_iterate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_parser_parse2_delegate(AVCodecParserContext* @s, AVCodecContext* @avctx, byte** @poutbuf, int* @poutbuf_size, byte* @buf, int @buf_size, long @pts, long @dts, long @pos);
    public static av_parser_parse2_delegate av_parser_parse2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_pix_fmt_count_planes_delegate(AVPixelFormat @pix_fmt);
    public static av_pix_fmt_count_planes_delegate av_pix_fmt_count_planes;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixFmtDescriptor* av_pix_fmt_desc_get_delegate(AVPixelFormat @pix_fmt);
    public static av_pix_fmt_desc_get_delegate av_pix_fmt_desc_get;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat av_pix_fmt_desc_get_id_delegate(AVPixFmtDescriptor* @desc);
    public static av_pix_fmt_desc_get_id_delegate av_pix_fmt_desc_get_id;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixFmtDescriptor* av_pix_fmt_desc_next_delegate(AVPixFmtDescriptor* @prev);
    public static av_pix_fmt_desc_next_delegate av_pix_fmt_desc_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_pix_fmt_get_chroma_sub_sample_delegate(AVPixelFormat @pix_fmt, int* @h_shift, int* @v_shift);
    public static av_pix_fmt_get_chroma_sub_sample_delegate av_pix_fmt_get_chroma_sub_sample;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat av_pix_fmt_swap_endianness_delegate(AVPixelFormat @pix_fmt);
    public static av_pix_fmt_swap_endianness_delegate av_pix_fmt_swap_endianness;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_pkt_dump_log2_delegate(void* @avcl, int @level, AVPacket* @pkt, int @dump_payload, AVStream* @st);
    public static av_pkt_dump_log2_delegate av_pkt_dump_log2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_pkt_dump2_delegate(_iobuf* @f, AVPacket* @pkt, int @dump_payload, AVStream* @st);
    public static av_pkt_dump2_delegate av_pkt_dump2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_probe_input_buffer_delegate(AVIOContext* @pb, AVInputFormat** @fmt,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, void* @logctx, uint @offset, uint @max_probe_size);
    public static av_probe_input_buffer_delegate av_probe_input_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_probe_input_buffer2_delegate(AVIOContext* @pb, AVInputFormat** @fmt,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, void* @logctx, uint @offset, uint @max_probe_size);
    public static av_probe_input_buffer2_delegate av_probe_input_buffer2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_probe_input_format_delegate(AVProbeData* @pd, int @is_opened);
    public static av_probe_input_format_delegate av_probe_input_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_probe_input_format2_delegate(AVProbeData* @pd, int @is_opened, int* @score_max);
    public static av_probe_input_format2_delegate av_probe_input_format2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVInputFormat* av_probe_input_format3_delegate(AVProbeData* @pd, int @is_opened, int* @score_ret);
    public static av_probe_input_format3_delegate av_probe_input_format3;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_program_add_stream_index_delegate(AVFormatContext* @ac, int @progid, uint @idx);
    public static av_program_add_stream_index_delegate av_program_add_stream_index;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_q2intfloat_delegate(AVRational @q);
    public static av_q2intfloat_delegate av_q2intfloat;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_read_frame_delegate(AVFormatContext* @s, AVPacket* @pkt);
    public static av_read_frame_delegate av_read_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_read_image_line_delegate(ushort* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component);
    public static av_read_image_line_delegate av_read_image_line;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_read_image_line2_delegate(void* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component, int @dst_element_size);
    public static av_read_image_line2_delegate av_read_image_line2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_read_pause_delegate(AVFormatContext* @s);
    public static av_read_pause_delegate av_read_pause;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_read_play_delegate(AVFormatContext* @s);
    public static av_read_play_delegate av_read_play;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_realloc_delegate(void* @ptr, ulong @size);
    public static av_realloc_delegate av_realloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_realloc_array_delegate(void* @ptr, ulong @nmemb, ulong @size);
    public static av_realloc_array_delegate av_realloc_array;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_realloc_f_delegate(void* @ptr, ulong @nelem, ulong @elsize);
    public static av_realloc_f_delegate av_realloc_f;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_reallocp_delegate(void* @ptr, ulong @size);
    public static av_reallocp_delegate av_reallocp;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_reallocp_array_delegate(void* @ptr, ulong @nmemb, ulong @size);
    public static av_reallocp_array_delegate av_reallocp_array;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_reduce_delegate(int* @dst_num, int* @dst_den, long @num, long @den, long @max);
    public static av_reduce_delegate av_reduce;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_rescale_delegate(long @a, long @b, long @c);
    public static av_rescale_delegate av_rescale;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_rescale_delta_delegate(AVRational @in_tb, long @in_ts, AVRational @fs_tb, int @duration, long* @last, AVRational @out_tb);
    public static av_rescale_delta_delegate av_rescale_delta;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_rescale_q_delegate(long @a, AVRational @bq, AVRational @cq);
    public static av_rescale_q_delegate av_rescale_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_rescale_q_rnd_delegate(long @a, AVRational @bq, AVRational @cq, AVRounding @rnd);
    public static av_rescale_q_rnd_delegate av_rescale_q_rnd;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_rescale_rnd_delegate(long @a, long @b, long @c, AVRounding @rnd);
    public static av_rescale_rnd_delegate av_rescale_rnd;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_sample_fmt_is_planar_delegate(AVSampleFormat @sample_fmt);
    public static av_sample_fmt_is_planar_delegate av_sample_fmt_is_planar;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_alloc_delegate(byte** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    public static av_samples_alloc_delegate av_samples_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_alloc_array_and_samples_delegate(byte*** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    public static av_samples_alloc_array_and_samples_delegate av_samples_alloc_array_and_samples;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_copy_delegate(byte** @dst, byte** @src, int @dst_offset, int @src_offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt);
    public static av_samples_copy_delegate av_samples_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_fill_arrays_delegate(byte** @audio_data, int* @linesize, byte* @buf, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    public static av_samples_fill_arrays_delegate av_samples_fill_arrays;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_get_buffer_size_delegate(int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    public static av_samples_get_buffer_size_delegate av_samples_get_buffer_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_samples_set_silence_delegate(byte** @audio_data, int @offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt);
    public static av_samples_set_silence_delegate av_samples_set_silence;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_sdp_create_delegate(AVFormatContext** @ac, int @n_files, byte* @buf, int @size);
    public static av_sdp_create_delegate av_sdp_create;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_seek_frame_delegate(AVFormatContext* @s, int @stream_index, long @timestamp, int @flags);
    public static av_seek_frame_delegate av_seek_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_set_options_string_delegate(void* @ctx,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @opts,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @key_val_sep,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @pairs_sep);
    public static av_set_options_string_delegate av_set_options_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_shrink_packet_delegate(AVPacket* @pkt, int @size);
    public static av_shrink_packet_delegate av_shrink_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_size_mult_delegate(ulong @a, ulong @b, ulong* @r);
    public static av_size_mult_delegate av_size_mult;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_strdup_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @s);
    public static av_strdup_delegate av_strdup;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_stream_add_side_data_delegate(AVStream* @st, AVPacketSideDataType @type, byte* @data, ulong @size);
    public static av_stream_add_side_data_delegate av_stream_add_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* av_stream_get_class_delegate();
    public static av_stream_get_class_delegate av_stream_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_stream_get_codec_timebase_delegate(AVStream* @st);
    public static av_stream_get_codec_timebase_delegate av_stream_get_codec_timebase;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long av_stream_get_end_pts_delegate(AVStream* @st);
    public static av_stream_get_end_pts_delegate av_stream_get_end_pts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecParserContext* av_stream_get_parser_delegate(AVStream* @s);
    public static av_stream_get_parser_delegate av_stream_get_parser;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_stream_get_side_data_delegate(AVStream* @stream, AVPacketSideDataType @type, ulong* @size);
    public static av_stream_get_side_data_delegate av_stream_get_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_stream_new_side_data_delegate(AVStream* @stream, AVPacketSideDataType @type, ulong @size);
    public static av_stream_new_side_data_delegate av_stream_new_side_data;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_strerror_delegate(int @errnum, byte* @errbuf, ulong @errbuf_size);
    public static av_strerror_delegate av_strerror;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_strndup_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @s, ulong @len);
    public static av_strndup_delegate av_strndup;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVRational av_sub_q_delegate(AVRational @b, AVRational @c);
    public static av_sub_q_delegate av_sub_q;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_tempfile_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @prefix, byte** @filename, int @log_offset, void* @log_ctx);
    public static av_tempfile_delegate av_tempfile;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_timecode_adjust_ntsc_framenum2_delegate(int @framenum, int @fps);
    public static av_timecode_adjust_ntsc_framenum2_delegate av_timecode_adjust_ntsc_framenum2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_timecode_check_frame_rate_delegate(AVRational @rate);
    public static av_timecode_check_frame_rate_delegate av_timecode_check_frame_rate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_timecode_get_smpte_delegate(AVRational @rate, int @drop, int @hh, int @mm, int @ss, int @ff);
    public static av_timecode_get_smpte_delegate av_timecode_get_smpte;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_timecode_get_smpte_from_framenum_delegate(AVTimecode* @tc, int @framenum);
    public static av_timecode_get_smpte_from_framenum_delegate av_timecode_get_smpte_from_framenum;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_timecode_init_delegate(AVTimecode* @tc, AVRational @rate, int @flags, int @frame_start, void* @log_ctx);
    public static av_timecode_init_delegate av_timecode_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_timecode_init_from_components_delegate(AVTimecode* @tc, AVRational @rate, int @flags, int @hh, int @mm, int @ss, int @ff, void* @log_ctx);
    public static av_timecode_init_from_components_delegate av_timecode_init_from_components;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_timecode_init_from_string_delegate(AVTimecode* @tc, AVRational @rate,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str, void* @log_ctx);
    public static av_timecode_init_from_string_delegate av_timecode_init_from_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_timecode_make_mpeg_tc_string_delegate(byte* @buf, uint @tc25bit);
    public static av_timecode_make_mpeg_tc_string_delegate av_timecode_make_mpeg_tc_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_timecode_make_smpte_tc_string_delegate(byte* @buf, uint @tcsmpte, int @prevent_df);
    public static av_timecode_make_smpte_tc_string_delegate av_timecode_make_smpte_tc_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_timecode_make_smpte_tc_string2_delegate(byte* @buf, AVRational @rate, uint @tcsmpte, int @prevent_df, int @skip_field);
    public static av_timecode_make_smpte_tc_string2_delegate av_timecode_make_smpte_tc_string2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* av_timecode_make_string_delegate(AVTimecode* @tc, byte* @buf, int @framenum);
    public static av_timecode_make_string_delegate av_timecode_make_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_tree_destroy_delegate(AVTreeNode* @t);
    public static av_tree_destroy_delegate av_tree_destroy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_tree_enumerate_delegate(AVTreeNode* @t, void* @opaque, av_tree_enumerate_cmp_func @cmp, av_tree_enumerate_enu_func @enu);
    public static av_tree_enumerate_delegate av_tree_enumerate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_tree_find_delegate(AVTreeNode* @root, void* @key, av_tree_find_cmp_func @cmp, ref void_ptrArray2 @next);
    public static av_tree_find_delegate av_tree_find;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* av_tree_insert_delegate(AVTreeNode** @rootp, void* @key, av_tree_insert_cmp_func @cmp, AVTreeNode** @next);
    public static av_tree_insert_delegate av_tree_insert;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVTreeNode* av_tree_node_alloc_delegate();
    public static av_tree_node_alloc_delegate av_tree_node_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_url_split_delegate(byte* @proto, int @proto_size, byte* @authorization, int @authorization_size, byte* @hostname, int @hostname_size, int* @port_ptr, byte* @path, int @path_size,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url);
    public static av_url_split_delegate av_url_split;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_usleep_delegate(uint @usec);
    public static av_usleep_delegate av_usleep;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string av_version_info_delegate();
    public static av_version_info_delegate av_version_info;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_vlog_delegate(void* @avcl, int @level,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt, byte* @vl);
    public static av_vlog_delegate av_vlog;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_write_frame_delegate(AVFormatContext* @s, AVPacket* @pkt);
    public static av_write_frame_delegate av_write_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_write_image_line_delegate(ushort* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w);
    public static av_write_image_line_delegate av_write_image_line;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void av_write_image_line2_delegate(void* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @src_element_size);
    public static av_write_image_line2_delegate av_write_image_line2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_write_trailer_delegate(AVFormatContext* @s);
    public static av_write_trailer_delegate av_write_trailer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_write_uncoded_frame_delegate(AVFormatContext* @s, int @stream_index, AVFrame* @frame);
    public static av_write_uncoded_frame_delegate av_write_uncoded_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int av_write_uncoded_frame_query_delegate(AVFormatContext* @s, int @stream_index);
    public static av_write_uncoded_frame_query_delegate av_write_uncoded_frame_query;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint av_xiphlacing_delegate(byte* @s, uint @v);
    public static av_xiphlacing_delegate av_xiphlacing;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_align_dimensions_delegate(AVCodecContext* @s, int* @width, int* @height);
    public static avcodec_align_dimensions_delegate avcodec_align_dimensions;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_align_dimensions2_delegate(AVCodecContext* @s, int* @width, int* @height, ref int_array8 @linesize_align);
    public static avcodec_align_dimensions2_delegate avcodec_align_dimensions2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecContext* avcodec_alloc_context3_delegate(AVCodec* @codec);
    public static avcodec_alloc_context3_delegate avcodec_alloc_context3;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVChromaLocation avcodec_chroma_pos_to_enum_delegate(int @xpos, int @ypos);
    public static avcodec_chroma_pos_to_enum_delegate avcodec_chroma_pos_to_enum;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_close_delegate(AVCodecContext* @avctx);
    public static avcodec_close_delegate avcodec_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avcodec_configuration_delegate();
    public static avcodec_configuration_delegate avcodec_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_decode_subtitle2_delegate(AVCodecContext* @avctx, AVSubtitle* @sub, int* @got_sub_ptr, AVPacket* @avpkt);
    public static avcodec_decode_subtitle2_delegate avcodec_decode_subtitle2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_default_execute_delegate(AVCodecContext* @c, avcodec_default_execute_func_func @func, void* @arg, int* @ret, int @count, int @size);
    public static avcodec_default_execute_delegate avcodec_default_execute;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_default_execute2_delegate(AVCodecContext* @c, avcodec_default_execute2_func_func @func, void* @arg, int* @ret, int @count);
    public static avcodec_default_execute2_delegate avcodec_default_execute2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_default_get_buffer2_delegate(AVCodecContext* @s, AVFrame* @frame, int @flags);
    public static avcodec_default_get_buffer2_delegate avcodec_default_get_buffer2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_default_get_encode_buffer_delegate(AVCodecContext* @s, AVPacket* @pkt, int @flags);
    public static avcodec_default_get_encode_buffer_delegate avcodec_default_get_encode_buffer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat avcodec_default_get_format_delegate(AVCodecContext* @s, AVPixelFormat* @fmt);
    public static avcodec_default_get_format_delegate avcodec_default_get_format;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecDescriptor* avcodec_descriptor_get_delegate(AVCodecID @id);
    public static avcodec_descriptor_get_delegate avcodec_descriptor_get;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecDescriptor* avcodec_descriptor_get_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avcodec_descriptor_get_by_name_delegate avcodec_descriptor_get_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecDescriptor* avcodec_descriptor_next_delegate(AVCodecDescriptor* @prev);
    public static avcodec_descriptor_next_delegate avcodec_descriptor_next;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_encode_subtitle_delegate(AVCodecContext* @avctx, byte* @buf, int @buf_size, AVSubtitle* @sub);
    public static avcodec_encode_subtitle_delegate avcodec_encode_subtitle;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_enum_to_chroma_pos_delegate(int* @xpos, int* @ypos, AVChromaLocation @pos);
    public static avcodec_enum_to_chroma_pos_delegate avcodec_enum_to_chroma_pos;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_fill_audio_frame_delegate(AVFrame* @frame, int @nb_channels, AVSampleFormat @sample_fmt, byte* @buf, int @buf_size, int @align);
    public static avcodec_fill_audio_frame_delegate avcodec_fill_audio_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVPixelFormat avcodec_find_best_pix_fmt_of_list_delegate(AVPixelFormat* @pix_fmt_list, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr);
    public static avcodec_find_best_pix_fmt_of_list_delegate avcodec_find_best_pix_fmt_of_list;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodec* avcodec_find_decoder_delegate(AVCodecID @id);
    public static avcodec_find_decoder_delegate avcodec_find_decoder;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodec* avcodec_find_decoder_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avcodec_find_decoder_by_name_delegate avcodec_find_decoder_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodec* avcodec_find_encoder_delegate(AVCodecID @id);
    public static avcodec_find_encoder_delegate avcodec_find_encoder;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodec* avcodec_find_encoder_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avcodec_find_encoder_by_name_delegate avcodec_find_encoder_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_flush_buffers_delegate(AVCodecContext* @avctx);
    public static avcodec_flush_buffers_delegate avcodec_flush_buffers;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_free_context_delegate(AVCodecContext** @avctx);
    public static avcodec_free_context_delegate avcodec_free_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avcodec_get_class_delegate();
    public static avcodec_get_class_delegate avcodec_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avcodec_get_frame_class_delegate();
    public static avcodec_get_frame_class_delegate avcodec_get_frame_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecHWConfig* avcodec_get_hw_config_delegate(AVCodec* @codec, int @index);
    public static avcodec_get_hw_config_delegate avcodec_get_hw_config;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_get_hw_frames_parameters_delegate(AVCodecContext* @avctx, AVBufferRef* @device_ref, AVPixelFormat @hw_pix_fmt, AVBufferRef** @out_frames_ref);
    public static avcodec_get_hw_frames_parameters_delegate avcodec_get_hw_frames_parameters;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avcodec_get_name_delegate(AVCodecID @id);
    public static avcodec_get_name_delegate avcodec_get_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avcodec_get_subtitle_rect_class_delegate();
    public static avcodec_get_subtitle_rect_class_delegate avcodec_get_subtitle_rect_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVMediaType avcodec_get_type_delegate(AVCodecID @codec_id);
    public static avcodec_get_type_delegate avcodec_get_type;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_is_open_delegate(AVCodecContext* @s);
    public static avcodec_is_open_delegate avcodec_is_open;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avcodec_license_delegate();
    public static avcodec_license_delegate avcodec_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_open2_delegate(AVCodecContext* @avctx, AVCodec* @codec, AVDictionary** @options);
    public static avcodec_open2_delegate avcodec_open2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecParameters* avcodec_parameters_alloc_delegate();
    public static avcodec_parameters_alloc_delegate avcodec_parameters_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_parameters_copy_delegate(AVCodecParameters* @dst, AVCodecParameters* @src);
    public static avcodec_parameters_copy_delegate avcodec_parameters_copy;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_parameters_free_delegate(AVCodecParameters** @par);
    public static avcodec_parameters_free_delegate avcodec_parameters_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_parameters_from_context_delegate(AVCodecParameters* @par, AVCodecContext* @codec);
    public static avcodec_parameters_from_context_delegate avcodec_parameters_from_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_parameters_to_context_delegate(AVCodecContext* @codec, AVCodecParameters* @par);
    public static avcodec_parameters_to_context_delegate avcodec_parameters_to_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avcodec_pix_fmt_to_codec_tag_delegate(AVPixelFormat @pix_fmt);
    public static avcodec_pix_fmt_to_codec_tag_delegate avcodec_pix_fmt_to_codec_tag;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avcodec_profile_name_delegate(AVCodecID @codec_id, int @profile);
    public static avcodec_profile_name_delegate avcodec_profile_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_receive_frame_delegate(AVCodecContext* @avctx, AVFrame* @frame);
    public static avcodec_receive_frame_delegate avcodec_receive_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_receive_packet_delegate(AVCodecContext* @avctx, AVPacket* @avpkt);
    public static avcodec_receive_packet_delegate avcodec_receive_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_send_frame_delegate(AVCodecContext* @avctx, AVFrame* @frame);
    public static avcodec_send_frame_delegate avcodec_send_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avcodec_send_packet_delegate(AVCodecContext* @avctx, AVPacket* @avpkt);
    public static avcodec_send_packet_delegate avcodec_send_packet;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avcodec_string_delegate(byte* @buf, int @buf_size, AVCodecContext* @enc, int @encode);
    public static avcodec_string_delegate avcodec_string;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avcodec_version_delegate();
    public static avcodec_version_delegate avcodec_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_app_to_dev_control_message_delegate(AVFormatContext* @s, AVAppToDevMessageType @type, void* @data, ulong @data_size);
    public static avdevice_app_to_dev_control_message_delegate avdevice_app_to_dev_control_message;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_capabilities_create_delegate(AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s, AVDictionary** @device_options);
    public static avdevice_capabilities_create_delegate avdevice_capabilities_create;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avdevice_capabilities_free_delegate(AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s);
    public static avdevice_capabilities_free_delegate avdevice_capabilities_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avdevice_configuration_delegate();
    public static avdevice_configuration_delegate avdevice_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_dev_to_app_control_message_delegate(AVFormatContext* @s, AVDevToAppMessageType @type, void* @data, ulong @data_size);
    public static avdevice_dev_to_app_control_message_delegate avdevice_dev_to_app_control_message;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avdevice_free_list_devices_delegate(AVDeviceInfoList** @device_list);
    public static avdevice_free_list_devices_delegate avdevice_free_list_devices;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avdevice_license_delegate();
    public static avdevice_license_delegate avdevice_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_list_devices_delegate(AVFormatContext* @s, AVDeviceInfoList** @device_list);
    public static avdevice_list_devices_delegate avdevice_list_devices;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_list_input_sources_delegate(AVInputFormat* @device,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list);
    public static avdevice_list_input_sources_delegate avdevice_list_input_sources;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avdevice_list_output_sinks_delegate(AVOutputFormat* @device,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list);
    public static avdevice_list_output_sinks_delegate avdevice_list_output_sinks;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avdevice_register_all_delegate();
    public static avdevice_register_all_delegate avdevice_register_all;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avdevice_version_delegate();
    public static avdevice_version_delegate avdevice_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_config_links_delegate(AVFilterContext* @filter);
    public static avfilter_config_links_delegate avfilter_config_links;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avfilter_configuration_delegate();
    public static avfilter_configuration_delegate avfilter_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avfilter_filter_pad_count_delegate(AVFilter* @filter, int @is_output);
    public static avfilter_filter_pad_count_delegate avfilter_filter_pad_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avfilter_free_delegate(AVFilterContext* @filter);
    public static avfilter_free_delegate avfilter_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilter* avfilter_get_by_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avfilter_get_by_name_delegate avfilter_get_by_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avfilter_get_class_delegate();
    public static avfilter_get_class_delegate avfilter_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilterGraph* avfilter_graph_alloc_delegate();
    public static avfilter_graph_alloc_delegate avfilter_graph_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilterContext* avfilter_graph_alloc_filter_delegate(AVFilterGraph* @graph, AVFilter* @filter,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avfilter_graph_alloc_filter_delegate avfilter_graph_alloc_filter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_config_delegate(AVFilterGraph* @graphctx, void* @log_ctx);
    public static avfilter_graph_config_delegate avfilter_graph_config;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_create_filter_delegate(AVFilterContext** @filt_ctx, AVFilter* @filt,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @args, void* @opaque, AVFilterGraph* @graph_ctx);
    public static avfilter_graph_create_filter_delegate avfilter_graph_create_filter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte* avfilter_graph_dump_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @options);
    public static avfilter_graph_dump_delegate avfilter_graph_dump;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avfilter_graph_free_delegate(AVFilterGraph** @graph);
    public static avfilter_graph_free_delegate avfilter_graph_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilterContext* avfilter_graph_get_filter_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avfilter_graph_get_filter_delegate avfilter_graph_get_filter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_parse_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filters, AVFilterInOut* @inputs, AVFilterInOut* @outputs, void* @log_ctx);
    public static avfilter_graph_parse_delegate avfilter_graph_parse;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_parse_ptr_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs, void* @log_ctx);
    public static avfilter_graph_parse_ptr_delegate avfilter_graph_parse_ptr;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_parse2_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs);
    public static avfilter_graph_parse2_delegate avfilter_graph_parse2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_queue_command_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @target,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @cmd,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @arg, int @flags, double @ts);
    public static avfilter_graph_queue_command_delegate avfilter_graph_queue_command;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_request_oldest_delegate(AVFilterGraph* @graph);
    public static avfilter_graph_request_oldest_delegate avfilter_graph_request_oldest;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_graph_send_command_delegate(AVFilterGraph* @graph,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @target,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @cmd,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @arg, byte* @res, int @res_len, int @flags);
    public static avfilter_graph_send_command_delegate avfilter_graph_send_command;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avfilter_graph_set_auto_convert_delegate(AVFilterGraph* @graph, uint @flags);
    public static avfilter_graph_set_auto_convert_delegate avfilter_graph_set_auto_convert;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_init_dict_delegate(AVFilterContext* @ctx, AVDictionary** @options);
    public static avfilter_init_dict_delegate avfilter_init_dict;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_init_str_delegate(AVFilterContext* @ctx,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @args);
    public static avfilter_init_str_delegate avfilter_init_str;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFilterInOut* avfilter_inout_alloc_delegate();
    public static avfilter_inout_alloc_delegate avfilter_inout_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avfilter_inout_free_delegate(AVFilterInOut** @inout);
    public static avfilter_inout_free_delegate avfilter_inout_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_insert_filter_delegate(AVFilterLink* @link, AVFilterContext* @filt, uint @filt_srcpad_idx, uint @filt_dstpad_idx);
    public static avfilter_insert_filter_delegate avfilter_insert_filter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avfilter_license_delegate();
    public static avfilter_license_delegate avfilter_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_link_delegate(AVFilterContext* @src, uint @srcpad, AVFilterContext* @dst, uint @dstpad);
    public static avfilter_link_delegate avfilter_link;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avfilter_link_free_delegate(AVFilterLink** @link);
    public static avfilter_link_free_delegate avfilter_link_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_pad_count_delegate(AVFilterPad* @pads);
    public static avfilter_pad_count_delegate avfilter_pad_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avfilter_pad_get_name_delegate(AVFilterPad* @pads, int @pad_idx);
    public static avfilter_pad_get_name_delegate avfilter_pad_get_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVMediaType avfilter_pad_get_type_delegate(AVFilterPad* @pads, int @pad_idx);
    public static avfilter_pad_get_type_delegate avfilter_pad_get_type;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avfilter_process_command_delegate(AVFilterContext* @filter,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @cmd,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @arg, byte* @res, int @res_len, int @flags);
    public static avfilter_process_command_delegate avfilter_process_command;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avfilter_version_delegate();
    public static avfilter_version_delegate avfilter_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVFormatContext* avformat_alloc_context_delegate();
    public static avformat_alloc_context_delegate avformat_alloc_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_alloc_output_context2_delegate(AVFormatContext** @ctx, AVOutputFormat* @oformat,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @format_name,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @filename);
    public static avformat_alloc_output_context2_delegate avformat_alloc_output_context2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avformat_close_input_delegate(AVFormatContext** @s);
    public static avformat_close_input_delegate avformat_close_input;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avformat_configuration_delegate();
    public static avformat_configuration_delegate avformat_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_find_stream_info_delegate(AVFormatContext* @ic, AVDictionary** @options);
    public static avformat_find_stream_info_delegate avformat_find_stream_info;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_flush_delegate(AVFormatContext* @s);
    public static avformat_flush_delegate avformat_flush;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avformat_free_context_delegate(AVFormatContext* @s);
    public static avformat_free_context_delegate avformat_free_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avformat_get_class_delegate();
    public static avformat_get_class_delegate avformat_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecTag* avformat_get_mov_audio_tags_delegate();
    public static avformat_get_mov_audio_tags_delegate avformat_get_mov_audio_tags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecTag* avformat_get_mov_video_tags_delegate();
    public static avformat_get_mov_video_tags_delegate avformat_get_mov_video_tags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecTag* avformat_get_riff_audio_tags_delegate();
    public static avformat_get_riff_audio_tags_delegate avformat_get_riff_audio_tags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVCodecTag* avformat_get_riff_video_tags_delegate();
    public static avformat_get_riff_video_tags_delegate avformat_get_riff_video_tags;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_index_get_entries_count_delegate(AVStream* @st);
    public static avformat_index_get_entries_count_delegate avformat_index_get_entries_count;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVIndexEntry* avformat_index_get_entry_delegate(AVStream* @st, int @idx);
    public static avformat_index_get_entry_delegate avformat_index_get_entry;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVIndexEntry* avformat_index_get_entry_from_timestamp_delegate(AVStream* @st, long @wanted_timestamp, int @flags);
    public static avformat_index_get_entry_from_timestamp_delegate avformat_index_get_entry_from_timestamp;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_init_output_delegate(AVFormatContext* @s, AVDictionary** @options);
    public static avformat_init_output_delegate avformat_init_output;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avformat_license_delegate();
    public static avformat_license_delegate avformat_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_match_stream_specifier_delegate(AVFormatContext* @s, AVStream* @st,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @spec);
    public static avformat_match_stream_specifier_delegate avformat_match_stream_specifier;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_network_deinit_delegate();
    public static avformat_network_deinit_delegate avformat_network_deinit;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_network_init_delegate();
    public static avformat_network_init_delegate avformat_network_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVStream* avformat_new_stream_delegate(AVFormatContext* @s, AVCodec* @c);
    public static avformat_new_stream_delegate avformat_new_stream;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_open_input_delegate(AVFormatContext** @ps,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, AVInputFormat* @fmt, AVDictionary** @options);
    public static avformat_open_input_delegate avformat_open_input;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_query_codec_delegate(AVOutputFormat* @ofmt, AVCodecID @codec_id, int @std_compliance);
    public static avformat_query_codec_delegate avformat_query_codec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_queue_attached_pictures_delegate(AVFormatContext* @s);
    public static avformat_queue_attached_pictures_delegate avformat_queue_attached_pictures;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_seek_file_delegate(AVFormatContext* @s, int @stream_index, long @min_ts, long @ts, long @max_ts, int @flags);
    public static avformat_seek_file_delegate avformat_seek_file;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_transfer_internal_stream_timing_info_delegate(AVOutputFormat* @ofmt, AVStream* @ost, AVStream* @ist, AVTimebaseSource @copy_tb);
    public static avformat_transfer_internal_stream_timing_info_delegate avformat_transfer_internal_stream_timing_info;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avformat_version_delegate();
    public static avformat_version_delegate avformat_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avformat_write_header_delegate(AVFormatContext* @s, AVDictionary** @options);
    public static avformat_write_header_delegate avformat_write_header;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_accept_delegate(AVIOContext* @s, AVIOContext** @c);
    public static avio_accept_delegate avio_accept;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVIOContext* avio_alloc_context_delegate(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque, avio_alloc_context_read_packet_func @read_packet, avio_alloc_context_write_packet_func @write_packet, avio_alloc_context_seek_func @seek);
    public static avio_alloc_context_delegate avio_alloc_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_check_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, int @flags);
    public static avio_check_delegate avio_check;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_close_delegate(AVIOContext* @s);
    public static avio_close_delegate avio_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_close_dir_delegate(AVIODirContext** @s);
    public static avio_close_dir_delegate avio_close_dir;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_close_dyn_buf_delegate(AVIOContext* @s, byte** @pbuffer);
    public static avio_close_dyn_buf_delegate avio_close_dyn_buf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_closep_delegate(AVIOContext** @s);
    public static avio_closep_delegate avio_closep;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_context_free_delegate(AVIOContext** @s);
    public static avio_context_free_delegate avio_context_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avio_enum_protocols_delegate(void** @opaque, int @output);
    public static avio_enum_protocols_delegate avio_enum_protocols;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_feof_delegate(AVIOContext* @s);
    public static avio_feof_delegate avio_feof;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avio_find_protocol_name_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url);
    public static avio_find_protocol_name_delegate avio_find_protocol_name;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_flush_delegate(AVIOContext* @s);
    public static avio_flush_delegate avio_flush;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_free_directory_entry_delegate(AVIODirEntry** @entry);
    public static avio_free_directory_entry_delegate avio_free_directory_entry;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_get_dyn_buf_delegate(AVIOContext* @s, byte** @pbuffer);
    public static avio_get_dyn_buf_delegate avio_get_dyn_buf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_get_str_delegate(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);
    public static avio_get_str_delegate avio_get_str;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_get_str16be_delegate(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);
    public static avio_get_str16be_delegate avio_get_str16be;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_get_str16le_delegate(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);
    public static avio_get_str16le_delegate avio_get_str16le;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_handshake_delegate(AVIOContext* @c);
    public static avio_handshake_delegate avio_handshake;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_open_delegate(AVIOContext** @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, int @flags);
    public static avio_open_delegate avio_open;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_open_dir_delegate(AVIODirContext** @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, AVDictionary** @options);
    public static avio_open_dir_delegate avio_open_dir;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_open_dyn_buf_delegate(AVIOContext** @s);
    public static avio_open_dyn_buf_delegate avio_open_dyn_buf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_open2_delegate(AVIOContext** @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @url, int @flags, AVIOInterruptCB* @int_cb, AVDictionary** @options);
    public static avio_open2_delegate avio_open2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_pause_delegate(AVIOContext* @h, int @pause);
    public static avio_pause_delegate avio_pause;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_print_string_array_delegate(AVIOContext* @s, byte*[] @strings);
    public static avio_print_string_array_delegate avio_print_string_array;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_printf_delegate(AVIOContext* @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt);
    public static avio_printf_delegate avio_printf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* avio_protocol_get_class_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name);
    public static avio_protocol_get_class_delegate avio_protocol_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_put_str_delegate(AVIOContext* @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str);
    public static avio_put_str_delegate avio_put_str;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_put_str16be_delegate(AVIOContext* @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str);
    public static avio_put_str16be_delegate avio_put_str16be;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_put_str16le_delegate(AVIOContext* @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @str);
    public static avio_put_str16le_delegate avio_put_str16le;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_r8_delegate(AVIOContext* @s);
    public static avio_r8_delegate avio_r8;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rb16_delegate(AVIOContext* @s);
    public static avio_rb16_delegate avio_rb16;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rb24_delegate(AVIOContext* @s);
    public static avio_rb24_delegate avio_rb24;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rb32_delegate(AVIOContext* @s);
    public static avio_rb32_delegate avio_rb32;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong avio_rb64_delegate(AVIOContext* @s);
    public static avio_rb64_delegate avio_rb64;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_read_delegate(AVIOContext* @s, byte* @buf, int @size);
    public static avio_read_delegate avio_read;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_read_dir_delegate(AVIODirContext* @s, AVIODirEntry** @next);
    public static avio_read_dir_delegate avio_read_dir;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_read_partial_delegate(AVIOContext* @s, byte* @buf, int @size);
    public static avio_read_partial_delegate avio_read_partial;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_read_to_bprint_delegate(AVIOContext* @h, AVBPrint* @pb, ulong @max_size);
    public static avio_read_to_bprint_delegate avio_read_to_bprint;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rl16_delegate(AVIOContext* @s);
    public static avio_rl16_delegate avio_rl16;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rl24_delegate(AVIOContext* @s);
    public static avio_rl24_delegate avio_rl24;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avio_rl32_delegate(AVIOContext* @s);
    public static avio_rl32_delegate avio_rl32;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong avio_rl64_delegate(AVIOContext* @s);
    public static avio_rl64_delegate avio_rl64;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long avio_seek_delegate(AVIOContext* @s, long @offset, int @whence);
    public static avio_seek_delegate avio_seek;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long avio_seek_time_delegate(AVIOContext* @h, int @stream_index, long @timestamp, int @flags);
    public static avio_seek_time_delegate avio_seek_time;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long avio_size_delegate(AVIOContext* @s);
    public static avio_size_delegate avio_size;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long avio_skip_delegate(AVIOContext* @s, long @offset);
    public static avio_skip_delegate avio_skip;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int avio_vprintf_delegate(AVIOContext* @s,     
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @fmt, byte* @ap);
    public static avio_vprintf_delegate avio_vprintf;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_w8_delegate(AVIOContext* @s, int @b);
    public static avio_w8_delegate avio_w8;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wb16_delegate(AVIOContext* @s, uint @val);
    public static avio_wb16_delegate avio_wb16;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wb24_delegate(AVIOContext* @s, uint @val);
    public static avio_wb24_delegate avio_wb24;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wb32_delegate(AVIOContext* @s, uint @val);
    public static avio_wb32_delegate avio_wb32;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wb64_delegate(AVIOContext* @s, ulong @val);
    public static avio_wb64_delegate avio_wb64;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wl16_delegate(AVIOContext* @s, uint @val);
    public static avio_wl16_delegate avio_wl16;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wl24_delegate(AVIOContext* @s, uint @val);
    public static avio_wl24_delegate avio_wl24;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wl32_delegate(AVIOContext* @s, uint @val);
    public static avio_wl32_delegate avio_wl32;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_wl64_delegate(AVIOContext* @s, ulong @val);
    public static avio_wl64_delegate avio_wl64;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_write_delegate(AVIOContext* @s, byte* @buf, int @size);
    public static avio_write_delegate avio_write;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avio_write_marker_delegate(AVIOContext* @s, long @time, AVIODataMarkerType @type);
    public static avio_write_marker_delegate avio_write_marker;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void avsubtitle_free_delegate(AVSubtitle* @sub);
    public static avsubtitle_free_delegate avsubtitle_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avutil_configuration_delegate();
    public static avutil_configuration_delegate avutil_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string avutil_license_delegate();
    public static avutil_license_delegate avutil_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint avutil_version_delegate();
    public static avutil_version_delegate avutil_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string postproc_configuration_delegate();
    public static postproc_configuration_delegate postproc_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string postproc_license_delegate();
    public static postproc_license_delegate postproc_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint postproc_version_delegate();
    public static postproc_version_delegate postproc_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void pp_free_context_delegate(void* @ppContext);
    public static pp_free_context_delegate pp_free_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void pp_free_mode_delegate(void* @mode);
    public static pp_free_mode_delegate pp_free_mode;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* pp_get_context_delegate(int @width, int @height, int @flags);
    public static pp_get_context_delegate pp_get_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void* pp_get_mode_by_name_and_quality_delegate(    
    #if NETSTANDARD2_1_OR_GREATER
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    #else
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    #endif
    string @name, int @quality);
    public static pp_get_mode_by_name_and_quality_delegate pp_get_mode_by_name_and_quality;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void pp_postprocess_delegate(in byte_ptrArray3 @src, in int_array3 @srcStride, ref byte_ptrArray3 @dst, in int_array3 @dstStride, int @horizontalSize, int @verticalSize, sbyte* @QP_store, int @QP_stride, void* @mode, void* @ppContext, int @pict_type);
    public static pp_postprocess_delegate pp_postprocess;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwrContext* swr_alloc_delegate();
    public static swr_alloc_delegate swr_alloc;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwrContext* swr_alloc_set_opts_delegate(SwrContext* @s, long @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, long @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx);
    public static swr_alloc_set_opts_delegate swr_alloc_set_opts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_alloc_set_opts2_delegate(SwrContext** @ps, AVChannelLayout* @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, AVChannelLayout* @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx);
    public static swr_alloc_set_opts2_delegate swr_alloc_set_opts2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_build_matrix_delegate(ulong @in_layout, ulong @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @rematrix_maxval, double @rematrix_volume, double* @matrix, int @stride, AVMatrixEncoding @matrix_encoding, void* @log_ctx);
    public static swr_build_matrix_delegate swr_build_matrix;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_build_matrix2_delegate(AVChannelLayout* @in_layout, AVChannelLayout* @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @maxval, double @rematrix_volume, double* @matrix, long @stride, AVMatrixEncoding @matrix_encoding, void* @log_context);
    public static swr_build_matrix2_delegate swr_build_matrix2;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void swr_close_delegate(SwrContext* @s);
    public static swr_close_delegate swr_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_config_frame_delegate(SwrContext* @swr, AVFrame* @out, AVFrame* @in);
    public static swr_config_frame_delegate swr_config_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_convert_delegate(SwrContext* @s, byte** @out, int @out_count, byte** @in, int @in_count);
    public static swr_convert_delegate swr_convert;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_convert_frame_delegate(SwrContext* @swr, AVFrame* @output, AVFrame* @input);
    public static swr_convert_frame_delegate swr_convert_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_drop_output_delegate(SwrContext* @s, int @count);
    public static swr_drop_output_delegate swr_drop_output;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void swr_free_delegate(SwrContext** @s);
    public static swr_free_delegate swr_free;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* swr_get_class_delegate();
    public static swr_get_class_delegate swr_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long swr_get_delay_delegate(SwrContext* @s, long @base);
    public static swr_get_delay_delegate swr_get_delay;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_get_out_samples_delegate(SwrContext* @s, int @in_samples);
    public static swr_get_out_samples_delegate swr_get_out_samples;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_init_delegate(SwrContext* @s);
    public static swr_init_delegate swr_init;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_inject_silence_delegate(SwrContext* @s, int @count);
    public static swr_inject_silence_delegate swr_inject_silence;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_is_initialized_delegate(SwrContext* @s);
    public static swr_is_initialized_delegate swr_is_initialized;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long swr_next_pts_delegate(SwrContext* @s, long @pts);
    public static swr_next_pts_delegate swr_next_pts;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_set_channel_mapping_delegate(SwrContext* @s, int* @channel_map);
    public static swr_set_channel_mapping_delegate swr_set_channel_mapping;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_set_compensation_delegate(SwrContext* @s, int @sample_delta, int @compensation_distance);
    public static swr_set_compensation_delegate swr_set_compensation;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int swr_set_matrix_delegate(SwrContext* @s, double* @matrix, int @stride);
    public static swr_set_matrix_delegate swr_set_matrix;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string swresample_configuration_delegate();
    public static swresample_configuration_delegate swresample_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string swresample_license_delegate();
    public static swresample_license_delegate swresample_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint swresample_version_delegate();
    public static swresample_version_delegate swresample_version;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsContext* sws_alloc_context_delegate();
    public static sws_alloc_context_delegate sws_alloc_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsVector* sws_allocVec_delegate(int @length);
    public static sws_allocVec_delegate sws_allocVec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_convertPalette8ToPacked24_delegate(byte* @src, byte* @dst, int @num_pixels, byte* @palette);
    public static sws_convertPalette8ToPacked24_delegate sws_convertPalette8ToPacked24;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_convertPalette8ToPacked32_delegate(byte* @src, byte* @dst, int @num_pixels, byte* @palette);
    public static sws_convertPalette8ToPacked32_delegate sws_convertPalette8ToPacked32;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_frame_end_delegate(SwsContext* @c);
    public static sws_frame_end_delegate sws_frame_end;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_frame_start_delegate(SwsContext* @c, AVFrame* @dst, AVFrame* @src);
    public static sws_frame_start_delegate sws_frame_start;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_freeContext_delegate(SwsContext* @swsContext);
    public static sws_freeContext_delegate sws_freeContext;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_freeFilter_delegate(SwsFilter* @filter);
    public static sws_freeFilter_delegate sws_freeFilter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_freeVec_delegate(SwsVector* @a);
    public static sws_freeVec_delegate sws_freeVec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate AVClass* sws_get_class_delegate();
    public static sws_get_class_delegate sws_get_class;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsContext* sws_getCachedContext_delegate(SwsContext* @context, int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param);
    public static sws_getCachedContext_delegate sws_getCachedContext;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int* sws_getCoefficients_delegate(int @colorspace);
    public static sws_getCoefficients_delegate sws_getCoefficients;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_getColorspaceDetails_delegate(SwsContext* @c, int** @inv_table, int* @srcRange, int** @table, int* @dstRange, int* @brightness, int* @contrast, int* @saturation);
    public static sws_getColorspaceDetails_delegate sws_getColorspaceDetails;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsContext* sws_getContext_delegate(int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param);
    public static sws_getContext_delegate sws_getContext;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsFilter* sws_getDefaultFilter_delegate(float @lumaGBlur, float @chromaGBlur, float @lumaSharpen, float @chromaSharpen, float @chromaHShift, float @chromaVShift, int @verbose);
    public static sws_getDefaultFilter_delegate sws_getDefaultFilter;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SwsVector* sws_getGaussianVec_delegate(double @variance, double @quality);
    public static sws_getGaussianVec_delegate sws_getGaussianVec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_init_context_delegate(SwsContext* @sws_context, SwsFilter* @srcFilter, SwsFilter* @dstFilter);
    public static sws_init_context_delegate sws_init_context;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_isSupportedEndiannessConversion_delegate(AVPixelFormat @pix_fmt);
    public static sws_isSupportedEndiannessConversion_delegate sws_isSupportedEndiannessConversion;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_isSupportedInput_delegate(AVPixelFormat @pix_fmt);
    public static sws_isSupportedInput_delegate sws_isSupportedInput;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_isSupportedOutput_delegate(AVPixelFormat @pix_fmt);
    public static sws_isSupportedOutput_delegate sws_isSupportedOutput;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_normalizeVec_delegate(SwsVector* @a, double @height);
    public static sws_normalizeVec_delegate sws_normalizeVec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_receive_slice_delegate(SwsContext* @c, uint @slice_start, uint @slice_height);
    public static sws_receive_slice_delegate sws_receive_slice;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint sws_receive_slice_alignment_delegate(SwsContext* @c);
    public static sws_receive_slice_alignment_delegate sws_receive_slice_alignment;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_scale_delegate(SwsContext* @c, byte*[] @srcSlice, int[] @srcStride, int @srcSliceY, int @srcSliceH, byte*[] @dst, int[] @dstStride);
    public static sws_scale_delegate sws_scale;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_scale_frame_delegate(SwsContext* @c, AVFrame* @dst, AVFrame* @src);
    public static sws_scale_frame_delegate sws_scale_frame;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sws_scaleVec_delegate(SwsVector* @a, double @scalar);
    public static sws_scaleVec_delegate sws_scaleVec;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_send_slice_delegate(SwsContext* @c, uint @slice_start, uint @slice_height);
    public static sws_send_slice_delegate sws_send_slice;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int sws_setColorspaceDetails_delegate(SwsContext* @c, in int_array4 @inv_table, int @srcRange, in int_array4 @table, int @dstRange, int @brightness, int @contrast, int @saturation);
    public static sws_setColorspaceDetails_delegate sws_setColorspaceDetails;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string swscale_configuration_delegate();
    public static swscale_configuration_delegate swscale_configuration;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public delegate string swscale_license_delegate();
    public static swscale_license_delegate swscale_license;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint swscale_version_delegate();
    public static swscale_version_delegate swscale_version;
    
}
